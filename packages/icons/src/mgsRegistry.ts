import type { Component } from 'vue'
import IconError from '~icons/aqlife/error-icon'
import IconHome from '~icons/aqlife/home-icon'
import IconLogin from '~icons/aqlife/login-icon'
import IconRegister from '~icons/aqlife/register-icon'
import IconMarkdown from '~icons/aqlife/markdown-icon'
import { MgsIconName } from './mgsIconName'

export const mgsIconRegistry: Record<MgsIconName, Component> = {
  [MgsIconName.Error]: IconError,
  [MgsIconName.Home]: IconHome,
  [MgsIconName.Login]: IconLogin,
  [MgsIconName.Register]: IconRegister,
  [MgsIconName.Markdown]:IconMarkdown
}
