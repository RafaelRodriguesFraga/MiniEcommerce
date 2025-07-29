#!/bin/bash

COMMIT_MSG_FILE="$1"
MAX_LENGTH=72  # Limite de caracteres para o título do commit

# Regex para Conventional Commits
REGEX="^(feat|fix|chore|docs|style|refactor|test|perf|ci|build|revert)(\([a-z0-9\-]+\))?: .+"

# Pega a primeira linha (título do commit)
COMMIT_TITLE=$(head -n 1 "$COMMIT_MSG_FILE")

# Verifica se o commit segue o padrão Conventional Commits
if ! echo "$COMMIT_TITLE" | grep -qE "$REGEX"; then
  echo "❌ Commit inválido!"
  echo "➡️  Use o padrão: <tipo>(<escopo>): <descrição>"
  echo "➡️  Exemplos válidos:"
  echo "   feat: adiciona nova funcionalidade"
  echo "   fix(login): corrige validação de senha"
  echo "➡️  Tipos permitidos: feat, fix, chore, docs, style, refactor, test, perf, ci, build, revert"
  exit 1
fi

# Verifica se o título do commit excede o limite de caracteres
if [ ${#COMMIT_TITLE} -gt $MAX_LENGTH ]; then
  echo "❌ Título do commit muito longo!"
  echo "➡️  Máximo permitido: $MAX_LENGTH caracteres"
  echo "➡️  Título atual (${#COMMIT_TITLE} caracteres): \"$COMMIT_TITLE\""
  exit 1
fi

echo "✅ Commit válido!"