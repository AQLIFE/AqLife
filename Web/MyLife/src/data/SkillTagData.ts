

const Skills:readonly string[] = ['Vue3', 'Vite','TypeScript', 'JavaScript', 'HTML5', 'CSS3', 'Node.js', 'Express','Mysql', 'MongoDB', 'MySQL', 'ASP.NET Core', 'C#', 'Python', 'Django', 'Git', 'Docker', 'Nginx', 'Linux', 'Elemnet-Plus'] as const;
type SkillType = typeof Skills[number];

const EColors: readonly string[] = ['primary', 'success', 'warning', 'danger', 'info'] as const;
type EColorType = typeof EColors[number];


interface ISkillConfig {
    skill: SkillType;
    color: EColorType;
}

class SkillManager {
    private skillRaw:readonly ISkillConfig[];

    constructor() {
        this.skillRaw = [
            { skill: 'Vue3', color: EColors[1] },
            { skill: 'Elemnet-Plus', color: EColors[0] },
            { skill: 'ASP.NET Core', color: 'RoyalBlue' },
            { skill: 'Vite', color: EColors[1] },
            { skill: 'Mysql', color: EColors[0] }
        ];
    }

    getSkillColor = (sName: string): string =>{return this.skillRaw.find(e=>e.skill===sName)?.color ?? EColors[0];};
    
    isEColor = (sColor:string):boolean =>EColors.includes(sColor);
}


const skillManager = new SkillManager();
export { Skills,skillManager };
