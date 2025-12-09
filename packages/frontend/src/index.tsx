import React from 'react';
import {
  automationCatalog,
  automationCapabilities,
  devtoolsBlueprint,
  devtoolsChecklist,
} from '@vizzio/core';

const SectionTitle: React.FC<{ title: string; icon?: string; subtitle?: string }>= ({
  title,
  icon,
  subtitle,
}) => (
  <header style={{ marginBottom: '0.75rem' }}>
    <h2 style={{ margin: 0, fontSize: '1.25rem' }}>
      {icon ? `${icon} ` : ''}
      {title}
    </h2>
    {subtitle && (
      <p style={{ margin: '0.25rem 0 0', color: '#5f6368', fontSize: '0.9rem' }}>{subtitle}</p>
    )}
  </header>
);

const Card: React.FC<{ title: string; description: string; footer?: React.ReactNode }> = ({
  title,
  description,
  footer,
}) => (
  <div
    style={{
      border: '1px solid #e0e0e0',
      borderRadius: '12px',
      padding: '1rem',
      background: '#fff',
      boxShadow: '0 1px 2px rgba(15, 23, 42, 0.08)',
      display: 'flex',
      flexDirection: 'column',
      gap: '0.5rem',
    }}
  >
    <strong style={{ fontSize: '1.05rem' }}>{title}</strong>
    <span style={{ color: '#4b5563', lineHeight: 1.4 }}>{description}</span>
    {footer && <div style={{ marginTop: 'auto', fontSize: '0.85rem', color: '#64748b' }}>{footer}</div>}
  </div>
);

const BlueprintSection: React.FC<typeof devtoolsBlueprint[number]> = ({
  title,
  summary,
  highlights,
}) => (
  <section style={{ marginBottom: '1.5rem' }}>
    <header>
      <h3 style={{ marginBottom: '0.35rem', fontSize: '1.1rem' }}>{title}</h3>
      <p style={{ margin: 0, color: '#52525b', lineHeight: 1.4 }}>{summary}</p>
    </header>
    <ul style={{ margin: '0.75rem 0 0', paddingLeft: '1.25rem', display: 'grid', gap: '0.4rem' }}>
      {highlights.map((highlight) => (
        <li key={highlight.title} style={{ color: '#404040', lineHeight: 1.45 }}>
          <strong>{highlight.title}:</strong> {highlight.description}
          {highlight.details && (
            <ul style={{ paddingLeft: '1.1rem', margin: '0.35rem 0' }}>
              {highlight.details.map((detail) => (
                <li key={detail} style={{ color: '#4b5563' }}>
                  {detail}
                </li>
              ))}
            </ul>
          )}
        </li>
      ))}
    </ul>
  </section>
);

export const App: React.FC = () => {
  return (
    <main
      style={{
        fontFamily: "'Inter', system-ui, sans-serif",
        display: 'grid',
        gridTemplateColumns: 'minmax(0,1fr)',
        gap: '2rem',
        padding: '2rem',
        background: '#f8fafc',
        color: '#0f172a',
      }}
    >
      <section>
        <SectionTitle
          title="Painel Supremo de Desenvolvimento"
          icon="🛠️"
          subtitle="Orquestra todas as capacidades da plataforma Automation + blueprint das DevTools"
        />
        <p style={{ color: '#475569', lineHeight: 1.55 }}>
          Este painel consolida os módulos de automação, integrações e operações de produto com o blueprint
          completo das DevTools modernas. Use como cockpit para acompanhar capacidades disponíveis, o que está
          em progresso e quais blocos técnicos formam sua ferramenta suprema.
        </p>
      </section>

      <section>
        <SectionTitle
          title="Catálogo de Automação"
          icon="⚙️"
          subtitle="Visão organizada por domínios, com status e pacotes correspondentes"
        />
        <div style={{ display: 'grid', gap: '1.25rem' }}>
          {automationCatalog.map((category) => (
            <div
              key={category.id}
              style={{
                border: '1px solid #dbeafe',
                borderRadius: '16px',
                padding: '1.25rem',
                background: '#ffffff',
              }}
            >
              <header style={{ marginBottom: '0.75rem' }}>
                <h3 style={{ margin: 0, fontSize: '1.15rem' }}>
                  {category.icon ? `${category.icon} ` : ''}
                  {category.label}
                </h3>
              </header>
              <div
                style={{
                  display: 'grid',
                  gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
                  gap: '1rem',
                }}
              >
                {category.capabilities.map((capability) => (
                  <Card
                    key={capability.id}
                    title={`${capability.name}`}
                    description={capability.description}
                    footer={
                      <span>
                        Status: <strong>{capability.status}</strong>
                        {capability.package ? ` · pacote ${capability.package}` : ''}
                      </span>
                    }
                  />
                ))}
              </div>
            </div>
          ))}
        </div>
      </section>

      <section>
        <SectionTitle
          title="Blueprint das DevTools"
          icon="🧭"
          subtitle="Resumo navegável do guia completo em docs/devtools-blueprint.md"
        />
        <div style={{ display: 'grid', gap: '1.5rem' }}>
          {devtoolsBlueprint.map((section) => (
            <BlueprintSection key={section.id} {...section} />
          ))}
        </div>
      </section>

      <section>
        <SectionTitle
          title="Checklist de Cobertura"
          icon="✅"
          subtitle="Itens para validar se o painel supremo está completo"
        />
        <ul
          style={{
            margin: 0,
            paddingLeft: '1.4rem',
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
            gap: '0.65rem 1.5rem',
          }}
        >
          {devtoolsChecklist.map((item) => (
            <li key={item} style={{ color: '#334155', lineHeight: 1.5 }}>
              <span role="img" aria-label="done" style={{ marginRight: '0.5rem' }}>
                ✔️
              </span>
              {item}
            </li>
          ))}
        </ul>
      </section>

      <section>
        <SectionTitle
          title="Mapa rápido"
          icon="🗺️"
          subtitle="Visão tabular de todas as capacidades com seus domínios"
        />
        <table
          style={{
            width: '100%',
            borderCollapse: 'collapse',
            background: '#fff',
            borderRadius: '12px',
            overflow: 'hidden',
            boxShadow: '0 1px 2px rgba(15, 23, 42, 0.08)',
          }}
        >
          <thead style={{ background: '#e2e8f0', textAlign: 'left' }}>
            <tr>
              <th style={{ padding: '0.75rem 1rem' }}>Domínio</th>
              <th style={{ padding: '0.75rem 1rem' }}>Capacidade</th>
              <th style={{ padding: '0.75rem 1rem' }}>Status</th>
              <th style={{ padding: '0.75rem 1rem' }}>Pacote</th>
              <th style={{ padding: '0.75rem 1rem' }}>Tags</th>
            </tr>
          </thead>
          <tbody>
            {automationCapabilities.map((capability) => (
              <tr key={capability.id} style={{ borderBottom: '1px solid #e2e8f0' }}>
                <td style={{ padding: '0.65rem 1rem', whiteSpace: 'nowrap' }}>
                  {capability.icon ? `${capability.icon} ` : ''}
                  {capability.category}
                </td>
                <td style={{ padding: '0.65rem 1rem' }}>{capability.name}</td>
                <td style={{ padding: '0.65rem 1rem', textTransform: 'capitalize' }}>{capability.status}</td>
                <td style={{ padding: '0.65rem 1rem', color: '#6366f1' }}>{capability.package ?? '—'}</td>
                <td style={{ padding: '0.65rem 1rem', color: '#0f172a' }}>
                  {capability.tags?.join(', ') ?? '—'}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    </main>
  );
};

export default App;
