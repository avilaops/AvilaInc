#!/usr/bin/env node

import { Command } from 'commander';
import chalk from 'chalk';
import { automationCatalog, automationCapabilities, devtoolsBlueprint, devtoolsChecklist } from '@vizzio/core';

const program = new Command();

program
  .name('vizzio')
  .description('🚀 Vizzio Automation Platform CLI')
  .version('1.0.0');

program
  .command('panel')
  .description('Visão geral do painel supremo de desenvolvimento')
  .action(() => {
    console.log(chalk.bold.cyan('\n🛠️  Painel Supremo de Desenvolvimento'));
    console.log(
      chalk.gray(
        '  Consolida automações, integrações e blueprint das DevTools em um cockpit único.'
      )
    );

    automationCatalog.forEach((category) => {
      console.log(chalk`\n {bold ${category.icon ?? '•'} ${category.label}}`);
      category.capabilities.forEach((capability) => {
        const statusColor =
          capability.status === 'available'
            ? chalk.green
            : capability.status === 'in-progress'
              ? chalk.yellow
              : chalk.gray;
        console.log(
          chalk`
  {cyanBright ▸ ${capability.name}}
    {white ${capability.description}}
    {magenta Pacote}: {magentaBright ${capability.package ?? '—'}}
    {magenta Status}: ${statusColor(capability.status)}${
      capability.tags?.length ? chalk`\n    {magenta Tags}: {white ${capability.tags.join(', ')}}` : ''
    }
`
        );
      });
    });
  });

program
  .command('devtools')
  .description('Resumo do blueprint das DevTools')
  .option('-c, --checklist', 'Exibir checklist de cobertura')
  .action((options: { checklist?: boolean }) => {
    console.log(chalk.bold.blue('\n🧭 Blueprint das DevTools'));
    console.log(
      chalk.gray('  Visão high-level. Consulte docs/devtools-blueprint.md para detalhes.')
    );

    devtoolsBlueprint.forEach((section) => {
      console.log(chalk`\n {bold ${section.title}}`);
      console.log(chalk.gray(`  ${section.summary}`));
      section.highlights.forEach((highlight) => {
        console.log(chalk`    {cyan - ${highlight.title}}: {white ${highlight.description}}`);
      });
    });

    if (options.checklist) {
      console.log(chalk.bold.green('\n✅ Checklist de Cobertura'));
      devtoolsChecklist.forEach((item) => console.log(chalk`  {green ✔} {white ${item}}`));
    }
  });

// Comando: workflow
program
  .command('workflow')
  .description('Gerenciar workflows')
  .action(() => {
    console.log(chalk.cyan('📋 Workflows:'));
    console.log(chalk.gray('  vizzio workflow list     - Listar workflows'));
    console.log(chalk.gray('  vizzio workflow create   - Criar novo workflow'));
    console.log(chalk.gray('  vizzio workflow run      - Executar workflow'));
  });

// Comando: email
program
  .command('email')
  .description('Gerenciar emails')
  .action(() => {
    console.log(chalk.cyan('📧 Emails:'));
    console.log(chalk.gray('  vizzio email templates  - Listar templates'));
    console.log(chalk.gray('  vizzio email send       - Enviar email'));
  });

// Comando: finance
program
  .command('finance')
  .description('Ferramentas financeiras')
  .action(() => {
    console.log(chalk.cyan('💰 Finance:'));
    console.log(chalk.gray('  vizzio finance invoice  - Gerar fatura'));
    console.log(chalk.gray('  vizzio finance expense  - Registrar despesa'));
  });

// Comando: shortcuts
program
  .command('shortcuts')
  .description('Gerenciar atalhos')
  .action(() => {
    console.log(chalk.cyan('⚡ Shortcuts:'));
    console.log(chalk.gray('  vizzio shortcuts list   - Listar atalhos'));
    console.log(chalk.gray('  vizzio shortcuts create - Criar atalho'));
  });

program
  .command('map')
  .description('Mapa tabular de capacidades e domínios')
  .action(() => {
    console.log(chalk.bold('\n🗺️  Mapa de Capacidades'));
    automationCapabilities.forEach((capability) => {
      console.log(
        chalk`
 {bold ${capability.category}} :: {cyan ${capability.name}}
   Status: {white ${capability.status}}
   Pacote: {magenta ${capability.package ?? '—'}}
   Tags: {gray ${capability.tags?.join(', ') ?? '—'}}
`
      );
    });
  });

program.parse();
