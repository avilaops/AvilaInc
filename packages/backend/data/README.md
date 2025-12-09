# Panel state storage

Este diretório armazena o arquivo `panel-state.json`, que contém o catálogo persistido de capacidades do painel supremo.

O arquivo é criado automaticamente na primeira execução do backend (`npm run dev:backend` ou `npm run build && node dist/index.js`).

> **Importante:** não comite dados sensíveis. Se precisar de seeds customizados, edite o arquivo gerado mantendo o formato JSON.
