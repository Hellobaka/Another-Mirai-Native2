using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Handlers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions
{
    /// <summary>
    /// 基于 <see cref="CommandAttribute"/> 特性的指令处理器抽象基类。
    /// 继承此类后，在方法上标注 <see cref="CommandAttribute"/> 即可自动注册为指令处理器。
    /// <para>此类已实现 <see cref="IGroupMessageHandler"/> 和 <see cref="IPrivateMessageHandler"/>，
    /// 框架会自动发现并注册，无需额外配置。</para>
    /// </summary>
    public abstract class CommandHandlerBase : IGroupMessageHandler, IPrivateMessageHandler
    {
        /// <summary>
        /// 当前实例的指令方法缓存。首次调度时通过反射构建，后续复用以避免重复扫描。
        /// </summary>
        private List<(MethodInfo Method, CommandAttribute Attr)>? _commandCache;

        /// <inheritdoc/>
        public virtual Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
        {
            return DispatchAsync(e.Message.Text, e, null, ct);
        }

        /// <inheritdoc/>
        public virtual Task<EventHandleResult> OnReceivePrivateMessageAsync(PrivateMessageContext e, CancellationToken ct)
        {
            return DispatchAsync(e.Message.Text, null, e, ct);
        }

        /// <summary>
        /// 当没有任何指令匹配群聊消息时调用。可在派生类中重写以实现自定义的回退逻辑。
        /// </summary>
        /// <param name="e">包含当前群聊消息事件信息的上下文。</param>
        /// <param name="ct">可用于取消操作的取消令牌。</param>
        /// <returns>事件的处理结果。默认返回 <see cref="EventHandleResult.Pass"/>。</returns>
        protected virtual Task<EventHandleResult> OnNoMatchAsync(GroupMessageContext e, CancellationToken ct)
        {
            return Task.FromResult(EventHandleResult.Pass);
        }

        /// <summary>
        /// 当没有任何指令匹配私聊消息时调用。可在派生类中重写以实现自定义的回退逻辑。
        /// </summary>
        /// <param name="e">包含当前私聊消息事件信息的上下文。</param>
        /// <param name="ct">可用于取消操作的取消令牌。</param>
        /// <returns>事件的处理结果。默认返回 <see cref="EventHandleResult.Pass"/>。</returns>
        protected virtual Task<EventHandleResult> OnNoMatchAsync(PrivateMessageContext e, CancellationToken ct)
        {
            return Task.FromResult(EventHandleResult.Pass);
        }

        /// <summary>
        /// 当指令参数的类型转换失败时调用。可在派生类中重写以实现自定义的错误处理逻辑，例如向用户回复格式提示。
        /// </summary>
        /// <param name="paramName">转换失败的参数名，对应正则表达式中的具名分组名称。</param>
        /// <param name="rawValue">从消息中提取到的原始字符串值。</param>
        /// <param name="targetType">期望转换到的目标类型。</param>
        protected virtual void OnParameterParseError(string paramName, string rawValue, Type targetType) { }

        /// <summary>
        /// 遍历当前实例上所有标注了 <see cref="CommandAttribute"/> 的方法，依次检查作用域和消息匹配条件，
        /// 将消息路由到第一个匹配的处理方法。若无任何方法匹配，则调用对应的
        /// <see cref="OnNoMatchAsync(GroupMessageContext, CancellationToken)"/> 或
        /// <see cref="OnNoMatchAsync(PrivateMessageContext, CancellationToken)"/>。
        /// </summary>
        /// <param name="messageText">待匹配的消息原文。</param>
        /// <param name="groupCtx">群消息上下文；若当前消息来自私聊则为 <see langword="null"/>。</param>
        /// <param name="privateCtx">私聊消息上下文；若当前消息来自群聊则为 <see langword="null"/>。</param>
        /// <param name="ct">可用于取消操作的取消令牌。</param>
        /// <returns>第一个匹配指令的处理结果，或无匹配时的回退结果。</returns>
        private Task<EventHandleResult> DispatchAsync(string messageText, GroupMessageContext? groupCtx, PrivateMessageContext? privateCtx, CancellationToken ct)
        {
            bool isGroup = groupCtx != null;

            foreach (var (method, attribute) in GetCommandMethods())
            {
                if (isGroup && attribute.Scope == MessageScope.Private)
                {
                    continue;
                }

                if (!isGroup && attribute.Scope == MessageScope.Group)
                {
                    continue;
                }

                var parameters = method.GetParameters();

                Task<EventHandleResult>? result = TryDispatch(method, attribute, parameters, messageText, groupCtx, privateCtx, ct);

                if (result != null)
                {
                    return result;
                }
            }

            return isGroup
                ? OnNoMatchAsync(groupCtx!, ct)
                : OnNoMatchAsync(privateCtx!, ct);
        }

        /// <summary>
        /// 尝试将消息与单个指令方法进行匹配并调用。
        /// 对于 <see cref="MatchMode.Regex"/> 模式，使用 <see cref="Regex.Match(string, string)"/> 执行匹配；
        /// 其他模式使用 <see cref="MatchMessage"/> 执行简单字符串匹配。
        /// 匹配成功后调用 <see cref="TryBuildArgs"/> 构造参数并通过反射调用目标方法。
        /// </summary>
        /// <param name="method">目标指令方法的反射信息。</param>
        /// <param name="attribute">该方法上标注的 <see cref="CommandAttribute"/>。</param>
        /// <param name="parameters">目标方法的参数列表。</param>
        /// <param name="messageText">待匹配的消息原文。</param>
        /// <param name="groupCtx">群消息上下文；来自私聊时为 <see langword="null"/>。</param>
        /// <param name="privateCtx">私聊消息上下文；来自群聊时为 <see langword="null"/>。</param>
        /// <param name="ct">可用于取消操作的取消令牌。</param>
        /// <returns>匹配成功时返回方法调用的 <see cref="Task{EventHandleResult}"/>；匹配失败时返回 <see langword="null"/>。</returns>
        private Task<EventHandleResult>? TryDispatch(MethodInfo method, CommandAttribute attribute, ParameterInfo[] parameters, string messageText, GroupMessageContext? groupCtx, PrivateMessageContext? privateCtx, CancellationToken ct)
        {
            Match? regexMatch = null;

            if (attribute.MatchMode == MatchMode.Regex)
            {
                regexMatch = Regex.Match(messageText, attribute.Template);
                if (!regexMatch.Success)
                {
                    return null;
                }
            }
            else
            {
                if (!MatchMessage(messageText, attribute))
                {
                    return null;
                }
            }

            if (!TryBuildArgs(parameters, regexMatch, groupCtx, privateCtx, ct, messageText, out object[] args))
            {
                return null;
            }

            return InvokeMethod(method, args);
        }

        /// <summary>
        /// 根据目标方法的参数列表构造反射调用所需的参数数组。
        /// </summary>
        /// <remarks>
        /// 参数按以下规则填充，且位置任意、均可省略：
        /// <list type="bullet">
        ///   <item><description><see cref="GroupMessageContext"/>：直接填入 <paramref name="groupCtx"/>。</description></item>
        ///   <item><description><see cref="PrivateMessageContext"/>：直接填入 <paramref name="privateCtx"/>。</description></item>
        ///   <item><description><see cref="CancellationToken"/>：直接填入 <paramref name="ct"/>。</description></item>
        ///   <item><description>其余类型视为指令参数，按参数名从 <paramref name="regexMatch"/> 的分组名称中提取，
        ///   并通过 <see cref="Convert.ChangeType(object, Type)"/> 转换为目标类型。</description></item>
        /// </list>
        /// </remarks>
        /// <param name="parameters">目标方法的参数列表。</param>
        /// <param name="regexMatch"><see cref="MatchMode.Regex"/> 模式下的正则匹配结果；非正则模式时为 <see langword="null"/>。</param>
        /// <param name="groupCtx">群消息上下文；来自私聊时为 <see langword="null"/>。</param>
        /// <param name="privateCtx">私聊消息上下文；来自群聊时为 <see langword="null"/>。</param>
        /// <param name="ct">可用于取消操作的取消令牌。</param>
        /// <param name="raw">指令的原始消息内容。</param>
        /// <param name="args">构造成功时输出可直接传入 <see cref="MethodBase.Invoke(object, object[])"/> 的参数数组；失败时输出空数组。</param>
        /// <returns>所有参数均成功填充时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
        private bool TryBuildArgs(ParameterInfo[] parameters, Match? regexMatch, GroupMessageContext? groupCtx, PrivateMessageContext? privateCtx, CancellationToken ct, string raw, out object[] args)
        {
            args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo param = parameters[i];
                Type paramType = param.ParameterType;

                if (paramType == typeof(GroupMessageContext))
                {
                    // 与特性设置的Scope不匹配
                    if (groupCtx == null)
                    {
                        return false;
                    }

                    args[i] = groupCtx;
                    continue;
                }
                if (paramType == typeof(PrivateMessageContext))
                {
                    // 与特性设置的Scope不匹配
                    if (privateCtx == null)
                    {
                        return false;
                    }

                    args[i] = privateCtx;
                    continue;
                }
                if (paramType == typeof(CancellationToken))
                {
                    args[i] = ct;
                    continue;
                }
                if (param.Name == "raw")
                {
                    args[i] = raw;
                    continue;
                }

                if (regexMatch == null)
                {
                    // 非 Regex 模式
                    return false;
                }

                string? paramName = param.Name;
                if (paramName == null)
                {
                    return false;
                }

                // 将指令参数填入对应数组
                Group group = regexMatch.Groups[paramName];
                if (!group.Success)
                {
                    return false;
                }

                try
                {
                    args[i] = Convert.ChangeType(group.Value, paramType);
                }
                catch
                {
                    OnParameterParseError(paramName, group.Value, paramType);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 调用指令方法并将其返回值统一封装为 <see cref="Task{EventHandleResult}"/>。
        /// </summary>
        /// <remarks>
        /// 支持以下返回类型：
        /// <list type="bullet">
        ///   <item><description><see cref="Task{EventHandleResult}"/>：直接返回。</description></item>
        ///   <item><description><see cref="Task"/>：异步等待完成后返回 <see cref="EventHandleResult.Block"/>。</description></item>
        ///   <item><description><see cref="EventHandleResult"/>：同步方法，包装为已完成的 Task 返回。</description></item>
        ///   <item><description><c>void</c>：同步方法，执行完成后返回 <see cref="EventHandleResult.Block"/>。</description></item>
        /// </list>
        /// </remarks>
        /// <param name="method">目标方法的反射信息。</param>
        /// <param name="args">将传入方法的参数数组。</param>
        /// <returns>表示指令处理结果的 <see cref="Task{EventHandleResult}"/>。</returns>
        private Task<EventHandleResult> InvokeMethod(MethodInfo method, object[] args)
        {
            object? returnValue = method.Invoke(this, args);

            if (method.ReturnType == typeof(void))
            {
                return Task.FromResult(EventHandleResult.Block);
            }
            if (method.ReturnType == typeof(Task))
            {
                return WrapTaskAsync((Task)returnValue!);
            }
            if (method.ReturnType == typeof(EventHandleResult))
            {
                return Task.FromResult((EventHandleResult)returnValue!);
            }
            // Task<EventHandleResult>
            return (Task<EventHandleResult>)returnValue!;
        }

        /// <summary>
        /// 等待无返回值的异步任务完成，并将结果包装为 <see cref="EventHandleResult.Block"/>。
        /// </summary>
        /// <param name="task">待等待的无返回值异步任务。</param>
        /// <returns>任务完成后返回 <see cref="EventHandleResult.Block"/> 的 <see cref="Task{EventHandleResult}"/>。</returns>
        private static async Task<EventHandleResult> WrapTaskAsync(Task task)
        {
            await task.ConfigureAwait(false);
            return EventHandleResult.Block;
        }

        /// <summary>
        /// 使用非正则匹配模式的指令，检查消息文本是否符合指令条件。
        /// </summary>
        /// <param name="text">待检查的消息原文。</param>
        /// <param name="attr">包含匹配模式和模板字符串的指令特性。</param>
        /// <returns>消息文本满足匹配条件时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
        private static bool MatchMessage(string text, CommandAttribute attr)
        {
            return attr.MatchMode switch
            {
                MatchMode.StartWith => text.StartsWith(attr.Template),
                MatchMode.EndWith => text.EndsWith(attr.Template),
                MatchMode.Contain => text.Contains(attr.Template),
                MatchMode.FullMatch => text == attr.Template,
                _ => false,
            };
        }

        /// <summary>
        /// 获取当前实例上所有标注了 <see cref="CommandAttribute"/> 的方法及其对应特性。
        /// 结果在首次调用时通过反射构建并缓存，后续调用直接返回缓存结果。
        /// </summary>
        /// <returns>包含方法反射信息与对应 <see cref="CommandAttribute"/> 的元组列表。</returns>
        /// <exception cref="InvalidOperationException">
        /// 当标注了 <see cref="CommandAttribute"/> 的方法返回类型不是 <see cref="Task{EventHandleResult}"/> 时抛出。
        /// </exception>
        private List<(MethodInfo, CommandAttribute)> GetCommandMethods()
        {
            if (_commandCache != null)
            {
                return _commandCache;
            }

            _commandCache = [];

            foreach (var method in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                foreach (CommandAttribute attr in method.GetCustomAttributes(typeof(CommandAttribute), true))
                {
                    if (method.ReturnType != typeof(Task<EventHandleResult>)
                        && method.ReturnType != typeof(Task)
                        && method.ReturnType != typeof(EventHandleResult)
                        && method.ReturnType != typeof(void))
                    {
                        throw new InvalidOperationException(
                            $"方法 {method.DeclaringType?.Name}.{method.Name} 标注了 CommandAttribute，" +
                            $"但其返回类型为 {method.ReturnType.Name}，" +
                            $"必须为 Task<EventHandleResult>、Task、EventHandleResult 或 void 之一。");
                    }

                    _commandCache.Add((method, attr));
                }
            }
            return _commandCache;
        }
    }
}