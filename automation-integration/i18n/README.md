# Internationalization (i18n)

This directory contains translation files for the GitHub Integration feature.

## Available Languages

- **en-US.json** - English (United States)
- **pt-BR.json** - Portuguese (Brazil)

## Structure

All translations are organized under the `github` key:

```json
{
  "github": {
    "title": "...",
    "tabs": { ... },
    "overview": { ... },
    "issues": { ... },
    "pulls": { ... },
    "branches": { ... },
    "files": { ... },
    "common": { ... },
    "actions": { ... }
  }
}
```

## Usage in React Components

```tsx
import { useTranslation } from 'react-i18next';

function GitHubIntegration() {
  const { t } = useTranslation();

  return (
    <div>
      <h1>{t('github.title')}</h1>
      <p>{t('github.subtitle')}</p>
    </div>
  );
}
```

## Adding New Languages

1. Create a new file: `<language-code>.json`
2. Copy the structure from `en-US.json`
3. Translate all strings
4. Update this README with the new language

## Translation Keys

### Main Sections
- `github.title` - Page title
- `github.subtitle` - Page subtitle
- `github.tabs.*` - Tab navigation labels

### Overview Tab
- `github.overview.stats.*` - Statistics labels
- `github.overview.noDescription` - Fallback for empty description

### Issues Tab
- `github.issues.title` - Section title
- `github.issues.empty` - Empty state message
- `github.issues.status.*` - Issue status labels
- `github.issues.filter.*` - Filter options

### Pull Requests Tab
- `github.pulls.title` - Section title
- `github.pulls.empty` - Empty state message
- `github.pulls.status.*` - PR status labels
- `github.pulls.filter.*` - Filter options

### Branches Tab
- `github.branches.title` - Section title
- `github.branches.empty` - Empty state message
- `github.branches.protected` - Protected badge label

### Files Tab
- `github.files.title` - Section title
- `github.files.empty` - Empty state message
- `github.files.directory` - Directory label
- `github.files.file` - File label

### Common
- `github.common.*` - Shared labels (loading, error, author, dates)

### Actions
- `github.actions.*` - Action button labels (create, edit, delete, etc.)

## Best Practices

1. **Keep keys consistent** across all language files
2. **Use semantic keys** that describe the content, not the text
3. **Avoid hardcoded strings** in components
4. **Test all languages** before deploying
5. **Use placeholders** for dynamic content: `t('message', { name: 'John' })`

## Contributing

When adding new features:
1. Add English translations first (en-US.json)
2. Add Portuguese translations (pt-BR.json)
3. Update component to use translation keys
4. Test language switching
