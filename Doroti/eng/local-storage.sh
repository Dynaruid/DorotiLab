#!/usr/bin/env bash

doroti_workspace_root() {
  local doroti_root="$1"
  local parent
  parent="$(cd "$doroti_root/.." && pwd)"
  if [[ -d "$parent/tools/Doroti.DartToCSharp" ]]; then
    printf '%s\n' "$parent"
  else
    printf '%s\n' "$doroti_root"
  fi
}

doroti_local_root() {
  local doroti_root="$1"
  local workspace_root configured root
  workspace_root="$(doroti_workspace_root "$doroti_root")"
  configured="${DOROTI_LOCAL_ROOT:-}"
  if [[ -z "$configured" ]]; then
    root="$workspace_root/.doroti"
  elif [[ "$configured" = /* ]]; then
    root="$configured"
  else
    root="$workspace_root/$configured"
  fi
  mkdir -p "$root"
  (cd "$root" && pwd)
}

new_doroti_temporary_directory() {
  local doroti_root="$1"
  local name="$2"
  if [[ ! "$name" =~ ^[A-Za-z0-9][A-Za-z0-9._-]*$ ]]; then
    echo "Invalid Doroti temporary directory name: $name" >&2
    return 2
  fi
  local local_root temporary_root
  local_root="$(doroti_local_root "$doroti_root")"
  temporary_root="$local_root/tmp"
  mkdir -p "$temporary_root"
  export DOROTI_LOCAL_ROOT="$local_root"
  mktemp -d "$temporary_root/$name.XXXXXXXX"
}

remove_doroti_temporary_directory() {
  local doroti_root="$1"
  local target="$2"
  local local_root temporary_root resolved
  local_root="$(doroti_local_root "$doroti_root")"
  temporary_root="$(cd "$local_root/tmp" && pwd)"
  resolved="$(cd "$target" && pwd)"
  case "$resolved" in
    "$temporary_root"/*) rm -rf -- "$resolved" ;;
    *) echo "Refusing to clean a path outside the Doroti temporary root: $resolved" >&2; return 2 ;;
  esac
}
