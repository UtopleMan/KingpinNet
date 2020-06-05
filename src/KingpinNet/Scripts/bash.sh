
_{{AppName}}_bash_suggest() {
    local cur prev opts base
    COMPREPLY=()
    cur="${COMP_WORDS[COMP_CWORD]}"
    opts=$( ./{{AppName}} suggest "${COMP_WORDS[@]:1:$COMP_CWORD}" )
    COMPREPLY=( $(compgen -W "${opts}" -- ${cur}) )
    return 0
}
complete -F _{{AppName}}_bash_suggest -o default {{AppName}}
