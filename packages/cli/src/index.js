#!/usr/bin/env node
"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const commander_1 = require("commander");
const chalk_1 = __importDefault(require("chalk"));
const core_1 = require("@vizzio/core");
const program = new commander_1.Command();
program
    .name('vizzio')
    .description('🚀 Vizzio Automation Platform CLI')
    .version('1.0.0');
program
    .command('panel')
    .description('Visão geral do painel supremo de desenvolvimento')
    .action(() => {
    console.log(chalk_1.default.bold.cyan('\n🛠️  Painel Supremo de Desenvolvimento'));
    console.log(chalk_1.default.gray('  Consolida automações, integrações e blueprint das DevTools em um cockpit único.'));
    core_1.automationCatalog.forEach((category) => {
        console.log((0, chalk_1.default) `\n {bold ${category.icon ?? '•'} ${category.label}}`);
        category.capabilities.forEach((capability) => {
            const statusColor = capability.status === 'available'
                ? chalk_1.default.green
                : capability.status === 'in-progress'
                    ? chalk_1.default.yellow
                    : chalk_1.default.gray;
            console.log((0, chalk_1.default) `
  {cyanBright ▸ ${capability.name}}
    {white ${capability.description}}
    {magenta Pacote}: {magentaBright ${capability.package ?? '—'}}
    {magenta Status}: ${statusColor(capability.status)}${capability.tags?.length ? (0, chalk_1.default) `\n    {magenta Tags}: {white ${capability.tags.join(', ')}}` : ''}
`);
        });
    });
});
program
    .command('devtools')
    .description('Resumo do blueprint das DevTools')
    .option('-c, --checklist', 'Exibir checklist de cobertura')
    .action((options) => {
    console.log(chalk_1.default.bold.blue('\n🧭 Blueprint das DevTools'));
    console.log(chalk_1.default.gray('  Visão high-level. Consulte docs/devtools-blueprint.md para detalhes.'));
    core_1.devtoolsBlueprint.forEach((section) => {
        console.log((0, chalk_1.default) `\n {bold ${section.title}}`);
        console.log(chalk_1.default.gray(`  ${section.summary}`));
        section.highlights.forEach((highlight) => {
            console.log((0, chalk_1.default) `    {cyan - ${highlight.title}}: {white ${highlight.description}}`);
        });
    });
    if (options.checklist) {
        console.log(chalk_1.default.bold.green('\n✅ Checklist de Cobertura'));
        core_1.devtoolsChecklist.forEach((item) => console.log((0, chalk_1.default) `  {green ✔} {white ${item}}`));
    }
});
// Comando: workflow
program
    .command('workflow')
    .description('Gerenciar workflows')
    .action(() => {
    console.log(chalk_1.default.cyan('📋 Workflows:'));
    console.log(chalk_1.default.gray('  vizzio workflow list     - Listar workflows'));
    console.log(chalk_1.default.gray('  vizzio workflow create   - Criar novo workflow'));
    console.log(chalk_1.default.gray('  vizzio workflow run      - Executar workflow'));
});
// Comando: email
program
    .command('email')
    .description('Gerenciar emails')
    .action(() => {
    console.log(chalk_1.default.cyan('📧 Emails:'));
    console.log(chalk_1.default.gray('  vizzio email templates  - Listar templates'));
    console.log(chalk_1.default.gray('  vizzio email send       - Enviar email'));
});
// Comando: finance
program
    .command('finance')
    .description('Ferramentas financeiras')
    .action(() => {
    console.log(chalk_1.default.cyan('💰 Finance:'));
    console.log(chalk_1.default.gray('  vizzio finance invoice  - Gerar fatura'));
    console.log(chalk_1.default.gray('  vizzio finance expense  - Registrar despesa'));
});
// Comando: shortcuts
program
    .command('shortcuts')
    .description('Gerenciar atalhos')
    .action(() => {
    console.log(chalk_1.default.cyan('⚡ Shortcuts:'));
    console.log(chalk_1.default.gray('  vizzio shortcuts list   - Listar atalhos'));
    console.log(chalk_1.default.gray('  vizzio shortcuts create - Criar atalho'));
});
program
    .command('map')
    .description('Mapa tabular de capacidades e domínios')
    .action(() => {
    console.log(chalk_1.default.bold('\n🗺️  Mapa de Capacidades'));
    core_1.automationCapabilities.forEach((capability) => {
        console.log((0, chalk_1.default) `
 {bold ${capability.category}} :: {cyan ${capability.name}}
   Status: {white ${capability.status}}
   Pacote: {magenta ${capability.package ?? '—'}}
   Tags: {gray ${capability.tags?.join(', ') ?? '—'}}
`);
    });
});
program.parse();
//# sourceMappingURL=index.js.map