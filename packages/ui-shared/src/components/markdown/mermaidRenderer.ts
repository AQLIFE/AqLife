import mermaid from 'mermaid'

let nextMermaidId = 0
let renderQueue: Promise<void> = Promise.resolve()

mermaid.initialize({
  startOnLoad: false,
  securityLevel: 'loose',
  theme: 'default',
  // 由组件自己显示错误兜底，不让 Mermaid 往 DOM 注入错误 SVG。
  suppressErrorRendering: true,
})

function enqueueRender<T>(
  task: () => Promise<T>,
): Promise<T> {
  const run = renderQueue.then(task, task)

  renderQueue = run.then(
    () => undefined,
    () => undefined,
  )

  return run
}

export function renderMermaid(
  code: string,
): Promise<string> {
  const chartId = `mermaid-aqlife-${++nextMermaidId}`

  return enqueueRender(async () => {
    const { svg } = await mermaid.render(chartId, code)

    if (!svg?.trim()) {
      throw new Error('Mermaid render returned an empty SVG')
    }

    return svg
  })
}
