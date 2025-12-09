export type CapabilityStatus =
  | 'planned'
  | 'in-progress'
  | 'available'
  | 'blocked'
  | 'deprecated';

export interface AutomationCapability {
  id: string;
  name: string;
  description: string;
  package?: string;
  docs?: string;
  tags?: string[];
  status: CapabilityStatus;
  owner?: string;
  progress?: number;
}

export interface AutomationCategory {
  id: string;
  label: string;
  icon?: string;
  capabilities: AutomationCapability[];
}

export const automationCatalog: AutomationCategory[] = [
  {
    id: 'automation-platform',
    label: 'Automation Platform',
    icon: '🧠',
    capabilities: [
      {
        id: 'workflows',
        name: 'Workflow Orchestration',
        description: 'Motor de workflows, agendamento e execução condicional',
        package: '@vizzio/workflows',
        status: 'planned',
        tags: ['orchestration', 'scheduler', 'rules-engine'],
      },
      {
        id: 'shortcuts',
        name: 'Shortcuts Runtime',
        description: 'Gestão de atalhos globais, bindings e ações rápidas',
        package: '@vizzio/shortcuts',
        status: 'planned',
        tags: ['shortcuts', 'productivity'],
      },
      {
        id: 'ai-assistant',
        name: 'AI Assistant',
        description: 'Assistente cognitivo para suportar tomadas de decisão',
        package: '@vizzio/ai-assistant',
        status: 'in-progress',
        tags: ['ai', 'copilot'],
        docs: 'packages/ai-assistant/README.md',
      },
    ],
  },
  {
    id: 'customer-ops',
    label: 'Customer Ops',
    icon: '🤝',
    capabilities: [
      {
        id: 'email-service',
        name: 'Email Service',
        description: 'Templates, envios e pipelines de email transacional',
        package: '@vizzio/email-service',
        status: 'available',
        tags: ['email', 'communications'],
        docs: 'packages/email-service',
      },
      {
        id: 'marketing-automation',
        name: 'Marketing Automation',
        description: 'Nutrição de leads, campanhas e journeys multicanal',
        package: '@vizzio/marketing-automation',
        status: 'planned',
        tags: ['marketing', 'campaigns'],
      },
      {
        id: 'integrations',
        name: 'External Integrations',
        description: 'Integrações com Salesforce, HubSpot, Slack e outros CRMs',
        package: '@vizzio/integrations',
        status: 'in-progress',
        tags: ['crm', 'api', 'sync'],
        docs: 'packages/integrations',
      },
    ],
  },
  {
    id: 'revenue-ops',
    label: 'Revenue Ops',
    icon: '💸',
    capabilities: [
      {
        id: 'finance-tools',
        name: 'Finance Tools',
        description: 'Faturas, cobranças, despesas e reconciliação',
        package: '@vizzio/finance-tools',
        status: 'available',
        tags: ['billing', 'expenses', 'stripe'],
        docs: 'packages/finance-tools',
      },
      {
        id: 'sales-pipeline',
        name: 'Sales Pipeline',
        description: 'CRM interno, etapas de funil e forecasting',
        package: '@vizzio/sales-pipeline',
        status: 'planned',
        tags: ['sales', 'crm'],
      },
    ],
  },
  {
    id: 'platform',
    label: 'Platform & Infra',
    icon: '⚙️',
    capabilities: [
      {
        id: 'backend-api',
        name: 'Backend API',
        description: 'API principal, autenticação e serviços core',
        package: '@vizzio/backend',
        status: 'in-progress',
        tags: ['api', 'express', 'microservices'],
        docs: 'packages/backend',
      },
      {
        id: 'frontend-dashboard',
        name: 'Frontend Dashboard',
        description: 'Painel unificado para operação dos módulos',
        package: '@vizzio/frontend',
        status: 'in-progress',
        tags: ['dashboard', 'nextjs'],
        docs: 'packages/frontend',
      },
      {
        id: 'devtools-core',
        name: 'DevTools Core',
        description: 'Blueprint e assets para o painel supremo de desenvolvimento',
        package: '@vizzio/core',
        status: 'available',
        tags: ['documentation', 'architecture'],
        docs: 'packages/core',
      },
    ],
  },
];

export const automationCapabilities = automationCatalog.flatMap((category) =>
  category.capabilities.map((capability) => ({
    category: category.label,
    icon: category.icon,
    ...capability,
  }))
);
