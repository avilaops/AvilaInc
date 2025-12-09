"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.App = void 0;
const jsx_runtime_1 = require("react/jsx-runtime");
const core_1 = require("@vizzio/core");
const SectionTitle = ({ title, icon, subtitle, }) => ((0, jsx_runtime_1.jsxs)("header", { style: { marginBottom: '0.75rem' }, children: [(0, jsx_runtime_1.jsxs)("h2", { style: { margin: 0, fontSize: '1.25rem' }, children: [icon ? `${icon} ` : '', title] }), subtitle && ((0, jsx_runtime_1.jsx)("p", { style: { margin: '0.25rem 0 0', color: '#5f6368', fontSize: '0.9rem' }, children: subtitle }))] }));
const Card = ({ title, description, footer, }) => ((0, jsx_runtime_1.jsxs)("div", { style: {
        border: '1px solid #e0e0e0',
        borderRadius: '12px',
        padding: '1rem',
        background: '#fff',
        boxShadow: '0 1px 2px rgba(15, 23, 42, 0.08)',
        display: 'flex',
        flexDirection: 'column',
        gap: '0.5rem',
    }, children: [(0, jsx_runtime_1.jsx)("strong", { style: { fontSize: '1.05rem' }, children: title }), (0, jsx_runtime_1.jsx)("span", { style: { color: '#4b5563', lineHeight: 1.4 }, children: description }), footer && (0, jsx_runtime_1.jsx)("div", { style: { marginTop: 'auto', fontSize: '0.85rem', color: '#64748b' }, children: footer })] }));
const BlueprintSection = ({ title, summary, highlights, }) => ((0, jsx_runtime_1.jsxs)("section", { style: { marginBottom: '1.5rem' }, children: [(0, jsx_runtime_1.jsxs)("header", { children: [(0, jsx_runtime_1.jsx)("h3", { style: { marginBottom: '0.35rem', fontSize: '1.1rem' }, children: title }), (0, jsx_runtime_1.jsx)("p", { style: { margin: 0, color: '#52525b', lineHeight: 1.4 }, children: summary })] }), (0, jsx_runtime_1.jsx)("ul", { style: { margin: '0.75rem 0 0', paddingLeft: '1.25rem', display: 'grid', gap: '0.4rem' }, children: highlights.map((highlight) => ((0, jsx_runtime_1.jsxs)("li", { style: { color: '#404040', lineHeight: 1.45 }, children: [(0, jsx_runtime_1.jsxs)("strong", { children: [highlight.title, ":"] }), " ", highlight.description, highlight.details && ((0, jsx_runtime_1.jsx)("ul", { style: { paddingLeft: '1.1rem', margin: '0.35rem 0' }, children: highlight.details.map((detail) => ((0, jsx_runtime_1.jsx)("li", { style: { color: '#4b5563' }, children: detail }, detail))) }))] }, highlight.title))) })] }));
const App = () => {
    return ((0, jsx_runtime_1.jsxs)("main", { style: {
            fontFamily: "'Inter', system-ui, sans-serif",
            display: 'grid',
            gridTemplateColumns: 'minmax(0,1fr)',
            gap: '2rem',
            padding: '2rem',
            background: '#f8fafc',
            color: '#0f172a',
        }, children: [(0, jsx_runtime_1.jsxs)("section", { children: [(0, jsx_runtime_1.jsx)(SectionTitle, { title: "Painel Supremo de Desenvolvimento", icon: "\uD83D\uDEE0\uFE0F", subtitle: "Orquestra todas as capacidades da plataforma Automation + blueprint das DevTools" }), (0, jsx_runtime_1.jsx)("p", { style: { color: '#475569', lineHeight: 1.55 }, children: "Este painel consolida os m\u00F3dulos de automa\u00E7\u00E3o, integra\u00E7\u00F5es e opera\u00E7\u00F5es de produto com o blueprint completo das DevTools modernas. Use como cockpit para acompanhar capacidades dispon\u00EDveis, o que est\u00E1 em progresso e quais blocos t\u00E9cnicos formam sua ferramenta suprema." })] }), (0, jsx_runtime_1.jsxs)("section", { children: [(0, jsx_runtime_1.jsx)(SectionTitle, { title: "Cat\u00E1logo de Automa\u00E7\u00E3o", icon: "\u2699\uFE0F", subtitle: "Vis\u00E3o organizada por dom\u00EDnios, com status e pacotes correspondentes" }), (0, jsx_runtime_1.jsx)("div", { style: { display: 'grid', gap: '1.25rem' }, children: core_1.automationCatalog.map((category) => ((0, jsx_runtime_1.jsxs)("div", { style: {
                                border: '1px solid #dbeafe',
                                borderRadius: '16px',
                                padding: '1.25rem',
                                background: '#ffffff',
                            }, children: [(0, jsx_runtime_1.jsx)("header", { style: { marginBottom: '0.75rem' }, children: (0, jsx_runtime_1.jsxs)("h3", { style: { margin: 0, fontSize: '1.15rem' }, children: [category.icon ? `${category.icon} ` : '', category.label] }) }), (0, jsx_runtime_1.jsx)("div", { style: {
                                        display: 'grid',
                                        gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
                                        gap: '1rem',
                                    }, children: category.capabilities.map((capability) => ((0, jsx_runtime_1.jsx)(Card, { title: `${capability.name}`, description: capability.description, footer: (0, jsx_runtime_1.jsxs)("span", { children: ["Status: ", (0, jsx_runtime_1.jsx)("strong", { children: capability.status }), capability.package ? ` · pacote ${capability.package}` : ''] }) }, capability.id))) })] }, category.id))) })] }), (0, jsx_runtime_1.jsxs)("section", { children: [(0, jsx_runtime_1.jsx)(SectionTitle, { title: "Blueprint das DevTools", icon: "\uD83E\uDDED", subtitle: "Resumo naveg\u00E1vel do guia completo em docs/devtools-blueprint.md" }), (0, jsx_runtime_1.jsx)("div", { style: { display: 'grid', gap: '1.5rem' }, children: core_1.devtoolsBlueprint.map((section) => ((0, jsx_runtime_1.jsx)(BlueprintSection, { ...section }, section.id))) })] }), (0, jsx_runtime_1.jsxs)("section", { children: [(0, jsx_runtime_1.jsx)(SectionTitle, { title: "Checklist de Cobertura", icon: "\u2705", subtitle: "Itens para validar se o painel supremo est\u00E1 completo" }), (0, jsx_runtime_1.jsx)("ul", { style: {
                            margin: 0,
                            paddingLeft: '1.4rem',
                            display: 'grid',
                            gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
                            gap: '0.65rem 1.5rem',
                        }, children: core_1.devtoolsChecklist.map((item) => ((0, jsx_runtime_1.jsxs)("li", { style: { color: '#334155', lineHeight: 1.5 }, children: [(0, jsx_runtime_1.jsx)("span", { role: "img", "aria-label": "done", style: { marginRight: '0.5rem' }, children: "\u2714\uFE0F" }), item] }, item))) })] }), (0, jsx_runtime_1.jsxs)("section", { children: [(0, jsx_runtime_1.jsx)(SectionTitle, { title: "Mapa r\u00E1pido", icon: "\uD83D\uDDFA\uFE0F", subtitle: "Vis\u00E3o tabular de todas as capacidades com seus dom\u00EDnios" }), (0, jsx_runtime_1.jsxs)("table", { style: {
                            width: '100%',
                            borderCollapse: 'collapse',
                            background: '#fff',
                            borderRadius: '12px',
                            overflow: 'hidden',
                            boxShadow: '0 1px 2px rgba(15, 23, 42, 0.08)',
                        }, children: [(0, jsx_runtime_1.jsx)("thead", { style: { background: '#e2e8f0', textAlign: 'left' }, children: (0, jsx_runtime_1.jsxs)("tr", { children: [(0, jsx_runtime_1.jsx)("th", { style: { padding: '0.75rem 1rem' }, children: "Dom\u00EDnio" }), (0, jsx_runtime_1.jsx)("th", { style: { padding: '0.75rem 1rem' }, children: "Capacidade" }), (0, jsx_runtime_1.jsx)("th", { style: { padding: '0.75rem 1rem' }, children: "Status" }), (0, jsx_runtime_1.jsx)("th", { style: { padding: '0.75rem 1rem' }, children: "Pacote" }), (0, jsx_runtime_1.jsx)("th", { style: { padding: '0.75rem 1rem' }, children: "Tags" })] }) }), (0, jsx_runtime_1.jsx)("tbody", { children: core_1.automationCapabilities.map((capability) => ((0, jsx_runtime_1.jsxs)("tr", { style: { borderBottom: '1px solid #e2e8f0' }, children: [(0, jsx_runtime_1.jsxs)("td", { style: { padding: '0.65rem 1rem', whiteSpace: 'nowrap' }, children: [capability.icon ? `${capability.icon} ` : '', capability.category] }), (0, jsx_runtime_1.jsx)("td", { style: { padding: '0.65rem 1rem' }, children: capability.name }), (0, jsx_runtime_1.jsx)("td", { style: { padding: '0.65rem 1rem', textTransform: 'capitalize' }, children: capability.status }), (0, jsx_runtime_1.jsx)("td", { style: { padding: '0.65rem 1rem', color: '#6366f1' }, children: capability.package ?? '—' }), (0, jsx_runtime_1.jsx)("td", { style: { padding: '0.65rem 1rem', color: '#0f172a' }, children: capability.tags?.join(', ') ?? '—' })] }, capability.id))) })] })] })] }));
};
exports.App = App;
exports.default = exports.App;
//# sourceMappingURL=index.js.map