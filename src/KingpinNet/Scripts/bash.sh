
_{{AppName}}_bash_complete() {
    local cur prev opts base
    COMPREPLY=()
    cur="${COMP_WORDS[COMP_CWORD]}"
    opts=$( ./{{AppName}} complete "${COMP_WORDS[@]:1:$COMP_CWORD}" )
    COMPREPLY=( $(compgen -W "${opts}" -- ${cur}) )
    return 0
}
complete -F _{{AppName}}_bash_complete -o default {{AppName}}
