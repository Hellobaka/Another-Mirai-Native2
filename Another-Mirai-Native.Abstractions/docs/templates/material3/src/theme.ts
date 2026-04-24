// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { html } from 'lit-html'
import { loc, options } from './helper'

export type Theme = 'light' | 'dark' | 'auto'

function setTheme(theme: Theme) {
  localStorage.setItem('theme', theme)
  if (theme === 'auto') {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
    document.documentElement.setAttribute('data-theme', prefersDark ? 'dark' : 'light')
  } else {
    document.documentElement.setAttribute('data-theme', theme)
  }
}

async function getDefaultTheme(): Promise<Theme> {
  return (localStorage.getItem('theme') as Theme) || (await options()).defaultTheme || 'auto'
}

export async function initTheme() {
  const theme = await getDefaultTheme()
  setTheme(theme)

  // Listen for system theme changes
  window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
    const currentTheme = localStorage.getItem('theme') as Theme
    if (currentTheme === 'auto' || !currentTheme) {
      document.documentElement.setAttribute('data-theme', e.matches ? 'dark' : 'light')
    }
  })

  // Initialize theme toggle button
  initThemeToggle()
}

function initThemeToggle() {
  const themeToggle = document.getElementById('theme-toggle')
  if (!themeToggle) return

  themeToggle.addEventListener('click', async () => {
    const currentTheme = (localStorage.getItem('theme') as Theme) || 'auto'
    const themes: Theme[] = ['light', 'dark', 'auto']
    const currentIndex = themes.indexOf(currentTheme)
    const nextIndex = (currentIndex + 1) % themes.length
    const nextTheme = themes[nextIndex]

    setTheme(nextTheme)
    updateThemeIcon(nextTheme)
  })
}

function updateThemeIcon(theme: Theme) {
  const themeToggle = document.getElementById('theme-toggle')
  if (!themeToggle) return

  // The CSS handles showing/hiding the correct icon based on data-theme attribute
  // This function can be extended for additional feedback if needed
}

export function onThemeChange(callback: (theme: 'light' | 'dark') => void) {
  return new MutationObserver(() => callback(getTheme()))
    .observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] })
}

export function getTheme(): 'light' | 'dark' {
  return document.documentElement.getAttribute('data-theme') as 'light' | 'dark'
}

export async function themePicker(refresh: () => void) {
  const theme = await getDefaultTheme()

  return html`
    <md-icon-button
      id="theme-toggle"
      aria-label="${loc('changeTheme')}"
      title="${loc('changeTheme')}">
      <span class="material-symbols-outlined theme-icon-light">light_mode</span>
      <span class="material-symbols-outlined theme-icon-dark">dark_mode</span>
      <span class="material-symbols-outlined theme-icon-auto">routine</span>
    </md-icon-button>
  `
}
