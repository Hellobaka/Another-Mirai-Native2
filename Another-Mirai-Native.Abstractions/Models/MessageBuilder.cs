using Another_Mirai_Native.Abstractions.Models.MessageItem;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Models
{
    public class MessageBuilder
    {
        public List<MessageItemBase> Items { get; set; }

        public string Build()
        {
            StringBuilder builder = new();
            foreach (var item in Items)
            {
                builder.Append(item.ToString());
            }

            return builder.ToString();
        }

        public MessageBuilder Text(string text)
        {
            Items.Add(new Text(text));
            return this;
        }

        public MessageBuilder Image(string filePath)
        {
            Items.Add(new Image(filePath: filePath));
            return this;
        }

        public MessageBuilder ImageHash(string hash)
        {
            Items.Add(new Image(hash: hash));
            return this;
        }

        public MessageBuilder Record(string filePath)
        {
            Items.Add(new Record(filePath));
            return this;
        }

        public MessageBuilder RecordHash(string hash)
        {
            Items.Add(new Record(hash));
            return this;
        }

        public MessageBuilder At(long qq)
        {
            Items.Add(new At(qq, false));
            return this;
        }

        public MessageBuilder AtAll()
        {
            Items.Add(new At(0, true));
            return this;
        }
    }
}
