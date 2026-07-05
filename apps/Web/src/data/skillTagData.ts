const skills: readonly string[] = [
  'Vue3',
  'Vite',
  'TypeScript',
  'JavaScript',
  'HTML5',
  'CSS3',
  'Node.js',
  'Express',
  'Mysql',
  'MongoDB',
  'MySQL',
  'ASP.NET Core',
  'C#',
  'Python',
  'Django',
  'Git',
  'Docker',
  'Nginx',
  'Linux',
  'Elemnet-Plus',
] as const

type SkillType = (typeof skills)[number]

const skillColors: readonly string[] = ['primary', 'success', 'warning', 'danger', 'info'] as const

interface SkillConfig {
  skill: SkillType
  color: (typeof skillColors)[number]
}

class SkillManager {
  private skillRaw: readonly SkillConfig[]

  constructor() {
    this.skillRaw = [
      { skill: 'Vue3', color: skillColors[1] },
      { skill: 'Elemnet-Plus', color: skillColors[0] },
      { skill: 'ASP.NET Core', color: 'RoyalBlue' },
      { skill: 'Vite', color: skillColors[1] },
      { skill: 'Mysql', color: skillColors[0] },
    ]
  }

  getSkillColor = (skillName: string): string =>
    this.skillRaw.find((entry) => entry.skill === skillName)?.color ?? skillColors[0]

  isSkillColor = (color: string): boolean => skillColors.includes(color)
}

export interface CodeInfo {
  codeType: string
  codeLines: string[]
}

const skillManager = new SkillManager()

export { skills, skillManager, skillColors }