export interface BlueprintBullet {
  title: string;
  description: string;
  details?: string[];
}

export interface BlueprintSection {
  id: string;
  title: string;
  summary: string;
  highlights: BlueprintBullet[];
}

export const devtoolsBlueprint: BlueprintSection[] = [
  {
    id: 'architecture',
    title: 'Arquitetura e camadas',
    summary:
      'Camadas que sustentam as DevTools modernas: UI rica, protocolos de depuração, agentes nos motores do navegador e infraestrutura de replay.',
    highlights: [
      {
        title: 'Interface de usuário',
        description:
          'Painéis em HTML/CSS/JS com gerenciamento de estado, virtualização de árvores DOM e UX dedicada.',
      },
      {
        title: 'Protocolos de depuração',
        description:
          'Suporte a CDP, WebDriver BiDi e protocolos remotos para instrumentar runtime JS, DOM, CSS, rede e segurança.',
      },
      {
        title: 'Agentes do motor',
        description:
          'Hooks internos em Blink/WebKit/Gecko e V8/SpiderMonkey para expor métricas e eventos com baixa latência.',
      },
      {
        title: 'Infra de armazenamento',
        description:
          'Traces, snapshots e buffers persistidos para gravação, download (HAR, JSON) e replay de sessões.',
      },
    ],
  },
  {
    id: 'panels',
    title: 'Painéis principais',
    summary:
      'Mapa das áreas core: Elements, Sources, Network, Performance, Memory, Application, Security, Recorder e Lighthouse.',
    highlights: [
      {
        title: 'Elements & Styles',
        description:
          'Inspeção DOM/CSS com diffs incrementais, cálculo de layout e ferramentas para grid/flex/a11y.',
      },
      {
        title: 'Sources & Console',
        description:
          'Depuração JS com breakpoints avançados, async stacks e REPL conectado via runtime.evaluate.',
      },
      {
        title: 'Network & Performance',
        description:
          'Waterfall, HAR, throttling, flame charts e métricas Web Vitals para obter gargalos.',
      },
      {
        title: 'Application & Security',
        description:
          'Visão de storage, service workers, manifestos, TLS, CSP e verificações de mixed content.',
      },
      {
        title: 'Recorder & Lighthouse',
        description:
          'Fluxos gravados exportáveis para Puppeteer/Playwright e auditorias automáticas.',
      },
    ],
  },
  {
    id: 'engines',
    title: 'Motores e algoritmos',
    summary:
      'Como DOM, JS e motores de profiling viabilizam inspeção e diagnósticos de performance.',
    highlights: [
      {
        title: 'Instrumentação DOM',
        description:
          'Diffs otimizados (ex.: DOMAgent::buildNode) notificam mutações para observers e DevTools.',
      },
      {
        title: 'Runtime JavaScript',
        description:
          'Breakpoints por offset de bytecode, inline cache rewriting e async hook chaining.',
      },
      {
        title: 'Profilers',
        description:
          'Sampling de CPU e análise de heap com árvores de dominadores e retentores.',
      },
      {
        title: 'Web Vitals e rede',
        description:
          'Heurísticas para LCP, FID, CLS e reconstrução de waterfalls com cálculos de throughput.',
      },
    ],
  },
  {
    id: 'tooling',
    title: 'Ferramentas auxiliares',
    summary:
      'Protocolos externos, CLIs, integrações de bundlers e suítes de performance/a11y.',
    highlights: [
      {
        title: 'Protocolos externos',
        description:
          'Automação via WebDriver BiDi, CDP, Safari Remote e clients como Playwright/Puppeteer.',
      },
      {
        title: 'CLI & automação',
        description:
          'Ferramentas como devtools-protocol, chrome-devtools-frontend, VS Code Debug.',
      },
      {
        title: 'Perf tooling',
        description:
          'Perfetto, Firefox Profiler, SpeedCurve e WebPageTest integrados ao pipeline.',
      },
      {
        title: 'Acessibilidade e segurança',
        description:
          'Motores Axe, Accessibility Insights, verificadores CSP e SRI.',
      },
    ],
  },
  {
    id: 'workflows',
    title: 'Workflows característicos',
    summary:
      'Passo a passo para debugging, profiling, auditoria PWA, acessibilidade e automação CI.',
    highlights: [
      {
        title: 'Debugging JS',
        description:
          'Breakpoints condicionais, inspeção de closures e live edit.',
      },
      {
        title: 'Diagnóstico de performance',
        description:
          'Trace, análise de frames longos e correlação com rede.',
      },
      {
        title: 'Perfis de memória',
        description:
          'Heap snapshot, dominadores e allocation timeline para localizar vazamentos.',
      },
      {
        title: 'Auditoria PWA e A11y',
        description:
          'Verifier de manifests, quotas, background sync e checagem de contraste.',
      },
      {
        title: 'Fluxos automatizados',
        description:
          'Recorder → exportação → regressão visual/perf em CI.',
      },
    ],
  },
  {
    id: 'observability',
    title: 'Observabilidade',
    summary:
      'Coleta de métricas, logging, captura de erros e trace categories customizadas.',
    highlights: [
      {
        title: 'Rendering overlay',
        description:
          'FPS, input delay e paint timings exibidos em tempo real.',
      },
      {
        title: 'Logging centralizado',
        description:
          'Console + painel Issues com níveis e captura de Promise rejections.',
      },
      {
        title: 'Trace personalizado',
        description:
          'Categorias customizadas para eventos específicos de produtos.',
      },
    ],
  },
  {
    id: 'extensibility',
    title: 'Extensibilidade',
    summary:
      'Como Chrome, Firefox e Edge oferecem APIs para novos painéis, sidebars e integrações IDE.',
    highlights: [
      {
        title: 'Chrome DevTools Extensions',
        description:
          'Manifesto devtools_page, APIs chrome.devtools.* e painéis customizados.',
      },
      {
        title: 'Firefox Toolbox API',
        description:
          'Carregamento via about:debugging e módulos em JS/React.',
      },
      {
        title: 'IDE integrations',
        description:
          'DevTools para VS Code, ndb, Replay.io para depuração avançada.',
      },
    ],
  },
  {
    id: 'quality',
    title: 'Testes e validação',
    summary:
      'Estratégias de testes unitários, integração, protocolo e regressões visuais.',
    highlights: [
      {
        title: 'Testes de frontend',
        description:
          'Karma/Mocha para Chrome DevTools frontend e golden screenshots.',
      },
      {
        title: 'Testes de protocolo',
        description:
          'WebDriver BiDi e fixtures para garantir compatibilidade cross-browser.',
      },
    ],
  },
];

export const devtoolsChecklist = [
  'Inspeção DOM/CSS',
  'Depuração JavaScript',
  'Monitoramento de rede',
  'Profiling de performance',
  'Análise de memória',
  'Ferramentas de storage/PWA',
  'Segurança e certificados',
  'Acessibilidade integrada',
  'Auditorias automatizadas',
  'Automação remota',
];
