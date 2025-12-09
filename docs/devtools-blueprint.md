# Blueprint das DevTools dos Navegadores

## 1. Visão macro e arquitetura em camadas
- **Interface de usuário (frontend DevTools)**: painéis escritos em HTML/CSS/JS, renderizados dentro do navegador ou em janelas separadas. Implementam lógica de orquestração, armazenamento de estados (Redux-like stores no Chrome) e renderização virtualizada de árvores grandes (DOM, networking).
- **Protocolos de depuração**: camada de transporte (WebSocket/TCP/USB) que fala com o motor do navegador. Destaques:
  - *Chrome DevTools Protocol (CDP)* e sucessores como *BiDi* (protocolo bidirecional padronizado pelo W3C) e *Firefox Remote Debugging Protocol*.
  - APIs para instrumentar runtime JS, DOM, CSS, rede, performance, memória, segurança e armazenamento.
- **Agentes de instrumentação no motor do navegador**: hooks que expõem o estado interno do mecanismo de renderização (Blink/WebKit/Gecko) e do motor JS (V8/SpiderMonkey/JavaScriptCore). São responsáveis por coletar eventos, métricas e artefatos.
- **Infra de armazenamento e replay**: buffers circulares, traces e snapshots persistidos em memória, disco ou endpoints remotos. Permite gravação/pausa, downloads (HAR, trace JSON) e reprodução de sessões.

## 2. Painéis e funcionalidades principais
| Painel / Módulo | Objetivo-chave | Ferramentas & fluxos | Algoritmos / Tecnologias de suporte |
| --- | --- | --- | --- |
| **Elements / Inspector** | Inspecionar DOM, CSS e layout | Edição em linha, Regras CSS, Computed, Layout/Flex/Grid, A11y | DOM snapshot incremental, diffs em árvore, mapping DOM ↔️ CSSOM, cálculo de layout e box model.
| **Styles / CSS** | Diagnóstico de estilos | Highlight, animações, cascade, overrides, @media | Resolução de cascade, parsing seletor, timeline de animação.
| **Sources / Debugger** | Depurar JavaScript | Breakpoints (condicionais, logpoints), Call Stack, Watches, Snippets | Source maps (VLQ decoding), Async Stack Traces, instrumentação do runtime JS, stepping scheduler.
| **Console** | Execução imediata, logging | Evaluation, logging formatado, warnings, grupos | REPL conectado via protocolo runtime.evaluate, serializers, drivers de promessas.
| **Network** | Monitorar requisições | HAR export, throttling, blocking, filtros, initiators | Captura de requests/headers/cookies, cálculo de waterfall, HAR 1.2 serializer, protocolo HTTP/2 frame parsing, HTTP cache heuristics.
| **Performance / Timeline** | Profiling de renderização | Gravação de frames, flame charts, frame-by-frame, Web Vitals | Rastreamento de eventos (tracing), Sampling CPU profiler, Layout shift quantização, métricas RUM.
| **Memory** | Leak hunting | Heap snapshot, allocation timeline, Sampling profiler | Analise de grafos de objetos, dominator tree, retentores, sampling aleatório de alocações.
| **Application / Storage** | Inspecionar storage e APIs web | IndexedDB, Cache Storage, Service Workers, manifests | Serialização binária, consulta IndexedDB, SW lifecycle hooks, auditoria PWA.
| **Security** | Políticas e certificados | TLS details, Mixed content, CSP violations | Validadores TLS, parsing de certificados, matching de policies CSP/COOP/COEP.
| **Recorder / User Flows** | Captura & replay de cenários | Gravação de interações, geração de scripts | Escuta de eventos DOM, heurísticas de seletor estável, exportação (Puppeteer/Playwright/Lighthouse).
| **Lighthouse / Audits** | Auditoria automatizada | Performance, A11y, SEO, Best Practices | CPU/Network emulado via lantern, DOM snapshots, trace analysis, scoring ponderado.
| **Sensors / Device Mode** | Emulação de dispositivos | Throttling CPU/Rede, geolocalização, orientações | Simulação de frame viewport, APIs virtualizadas (Geolocation, Device Orientation).
| **Coverage & Code Inspector** | Otimização de bundles | Análise de cobertura JS/CSS, code flow | Instrumentação bytecode, counters em AST, análise viva de módulos.
| **Accessibility Tree** | Diagnóstico a11y | ARIA, roles, contrast checker | Construção da Accessibility Tree, algoritmos de contraste WCAG, mapear eventos AX.

## 3. Motores e algoritmos fundamentais
- **Instrumentação do DOM**: listeners internos notificam mutações (Mutation Agents) antes de dispatch aos observers. Utilizam diffs otimizados (ex.: `DOMAgent::buildNode` em Blink) para enviar apenas nós alterados.
- **Tempo de execução JavaScript**: breakpoints são geridos por bytecode offset e inline cache rewriting. Pilhas assíncronas preservadas via *async hook chaining*.
- **Profiling de CPU**: amostragem (sampling) periódica do ponteiro de instrução, agregação em árvores de chamada (flame charts). Apoiado por *V8 CPU Profiler* e *SpiderMonkey Gecko Profiler*.
- **Heap snapshotting**: representação de grafo com nós (objetos, closures, arrays) e arestas (referências). Algoritmos de dominadores e retentores para detectar vazamentos.
- **Coleta de métricas Web Vitals**: *Largest Contentful Paint*, *First Input Delay* e *Cumulative Layout Shift* calculados via timeline de eventos e heurísticas de atribuição.
- **Análise de rede**: reconstrução de waterfalls e dependências via gráficos direcionados (initiators). Cálculo de velocidade efetiva com base em TTFB, RTT, throughput.
- **Timelines de renderização**: pipeline frame (Input → Scripting → Style → Layout → Paint → Composite). Eventos instrumentados para medir custos e gargalos.
- **Source maps e empacotadores**: decodificação VLQ, merge de mapas, mapping “inverse” para highlight de bundles minimizados.
- **Algoritmos de comparação visual**: highlight de layout computa caixas de bounding e transformações; overlay de Grid/Flex calcula linhas virtuais.

## 4. Ferramentas auxiliares e integrações
- **Protocolos externos**: WebDriver BiDi, CDP, Remote Debugging, Safari Web Inspector Remote. Permitem orquestração por Playwright, Puppeteer, Selenium.
- **CLI & automação**: `chrome-devtools-frontend`, `devtools-protocol` npm, `vscode-chrome-debug`, *Remote Target Discovery* via JSON.
- **Perf tooling**: `about://tracing`, Perfetto, Firefox Profiler, SpeedCurve, WebPageTest integração com Lighthouse.
- **Build & bundlers**: integração com sourcemap consumers (Webpack, Rollup, ESBuild) e coverage reporters (Istanbul, V8 native coverage).
- **Acessibilidade**: motores AX integrados com ferramentas como Axe, Accessibility Insights, DevTools A11y audits.
- **Segurança**: verificadores de CSP, Mixed Content, Subresource Integrity, Reporting API, Security Panel hooking em certificados (OpenSSL analisadores internos).

## 5. Fluxos de trabalho característicos
1. **Debugging JS passo a passo**: breakpoints condicionais, stepping, inspeção de closures e variáveis, override de função com Live Edit.
2. **Diagnóstico de performance**: gravação de trace, análise de frames longos, correlação com eventos de rede, simulação de throttling.
3. **Perfis de memória**: heap snapshot → comparação diferenciada → identificar dominadores → desalocar → validar com allocation timeline.
4. **Otimização de carregamento**: uso do Painel Network + Lighthouse para revisar prioridades, caches e compressão.
5. **Auditoria PWA**: verificação de Service Worker, manifestos, storage quotas, background sync.
6. **Acessibilidade**: Accessibility tree, contrast, pane de ARIA, audit run com Axe/Lighthouse.
7. **Fluxos automatizados**: Recorder → exportação (Puppeteer/Playwright) → integração CI para regressão visual/de performance.

## 6. Observabilidade & Telemetria interna
- Coleta contínua de métricas (FPS, input delay, paint timings) exibida na aba Rendering.
- Hooks de logging com níveis (Verbose, Info, Warning, Error) integrados ao Console e painel Issues.
- Captura de erros de runtime/promise rejection com stack trace mapeado.
- Suporte a *trace categories* customizadas e registro de eventos do usuário.

## 7. Extensibilidade e personalização
- **Chrome**: Extensões de DevTools (painéis personalizados, sidebars) via manifesto `devtools_page` e APIs `chrome.devtools.*`.
- **Firefox**: Ferramentas personalizadas com *Toolbox API* e `about:debugging` para carregamento.
- **Edge**: Suporte a extensões estilo Chrome + integrações enterprise (DevTools for VS Code).
- **Plugins de automação**: integrações com IDEs e CLI (VS Code Debugger, JetBrains, ndb, Replay.io).

## 8. Roteiro de testes e validação
- Testes unitários e de integração executados nos frontends (Karma/Mocha no Chrome DevTools frontend).
- Testes de protocolo (WPT WebDriver BiDi, fixtures específicos) garantem compatibilidade cross-browser.
- Regressões monitoradas com `ci/devtools-frontend` (Chrome) e `mach test devtools` (Firefox).
- Comparação visual e *golden screenshots* para detectar regressões UI.

## 9. Checklist de cobertura cross-browser
- [x] Suporte a inspeção DOM/CSS
- [x] Depuração JavaScript
- [x] Monitoramento de rede
- [x] Profiling de performance
- [x] Análise de memória
- [x] Ferramentas de armazenamento/PWA
- [x] Segurança & certificados
- [x] Acessibilidade integrada
- [x] Auditorias automatizadas (Lighthouse/Accessibility Inspector)
- [x] Automação & scripting remoto

> **Resumo**: As DevTools modernas são um ecossistema composto por uma UI rica, protocolos de depuração padronizados e uma malha de agentes internos que expõem quase toda a superfície do browser. O blueprint acima destaca como os navegadores líderes estruturam suas ferramentas para inspeção, diagnóstico, performance, segurança, acessibilidade e automação.
