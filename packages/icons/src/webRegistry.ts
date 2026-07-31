import type { Component } from 'vue'
import IconAbout from '~icons/aqlife/about'
import IconBlog from '~icons/aqlife/blog'
import IconError from '~icons/aqlife/error-icon'
import IconHome from '~icons/aqlife/home-icon'
import IconPlan from '~icons/aqlife/plan-icon'
import IconRandom from '~icons/aqlife/random-icon'
import IconShare from '~icons/aqlife/share-icon'
import IconSkill from '~icons/aqlife/skill-icon'
import IconWish from '~icons/aqlife/wish-icon'
import IconKey from '~icons/aqlife/key-icon'
import IconMarkdown from '~icons/aqlife/markdown-icon'
import { WebIconName } from './webIconName'

export const webIconRegistry: Record<WebIconName, Component> = {
  [WebIconName.Error]: IconError,
  [WebIconName.Home]: IconHome,
  [WebIconName.About]: IconAbout,
  [WebIconName.Share]: IconShare,
  [WebIconName.Blog]: IconBlog,
  [WebIconName.Plan]: IconPlan,
  [WebIconName.Wish]: IconWish,
  [WebIconName.Random]: IconRandom,
  [WebIconName.Skill]: IconSkill,
  [WebIconName.Key]: IconKey,
  [WebIconName.Markdown]:IconMarkdown
}
