// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { options } from './helper'
import { highlight } from './highlight'
import { renderMarkdown } from './markdown'
import { enableSearch } from './search'
import { renderToc } from './toc'
import { initTheme } from './theme'
import { renderBreadcrumb, renderInThisArticle, renderNavbar } from './nav'

import './docfx.scss'

declare global {
  interface Window {
    docfx: {
      ready?: boolean,
      searchReady?: boolean,
      searchResultReady?: boolean,
    }
  }
}

async function init() {
  window.docfx = window.docfx || {}

  const { start } = await options()
  start?.()

  const pdfmode = navigator.userAgent.indexOf('docfx/pdf') >= 0
  if (pdfmode) {
    await Promise.all([
      renderMarkdown(),
      highlight()
    ])
  } else {
    await Promise.all([
      initTheme(),
      enableSearch(),
      renderInThisArticle(),
      renderMarkdown(),
      renderNav(),
      highlight(),
      initMobileNav()
    ])
  }

  window.docfx.ready = true

  async function renderNav() {
    const [navbar, toc] = await Promise.all([renderNavbar(), renderToc()])
    renderBreadcrumb([...navbar, ...toc])
  }

  // Initialize mobile navigation drawer
  function initMobileNav() {
    const navToggle = document.getElementById('nav-toggle')
    const navDrawer = document.getElementById('nav-drawer')
    const navOverlay = document.querySelector('.md-nav-overlay')

    if (!navToggle || !navDrawer) return

    // Create overlay element
    let overlay = navOverlay as HTMLElement
    if (!overlay) {
      overlay = document.createElement('div')
      overlay.className = 'md-nav-overlay'
      document.body.appendChild(overlay)
    }

    const toggleNav = (open: boolean) => {
      if (open) {
        navDrawer.classList.add('open')
        overlay.classList.add('visible')
        document.body.style.overflow = 'hidden'
      } else {
        navDrawer.classList.remove('open')
        overlay.classList.remove('visible')
        document.body.style.overflow = ''
      }
    }

    navToggle.addEventListener('click', () => {
      const isOpen = navDrawer.classList.contains('open')
      toggleNav(!isOpen)
    })

    overlay.addEventListener('click', () => {
      toggleNav(false)
    })

    // Close on escape key
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape' && navDrawer.classList.contains('open')) {
        toggleNav(false)
      }
    })

    // Close on nav link click (mobile)
    navDrawer.querySelectorAll('a').forEach(link => {
      link.addEventListener('click', () => {
        if (window.innerWidth <= 900) {
          toggleNav(false)
        }
      })
    })

    // Handle window resize
    let resizeTimeout: number
    window.addEventListener('resize', () => {
      clearTimeout(resizeTimeout)
      resizeTimeout = window.setTimeout(() => {
        if (window.innerWidth > 900) {
          toggleNav(false)
        }
      }, 100)
    })
  }
}

init().catch(console.error)
