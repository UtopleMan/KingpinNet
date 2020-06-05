#compdef {{AppName}}
_{{AppName}}() {
    local matches=($(${words[1]} suggest "${(@)words[1,$CURRENT]}"))
    compadd -a matches
    if [[ $compstate[nmatches] -eq 0 && $words[$CURRENT] != -* ]]; then
        _files
    fi
}
if [[ "$(basename -- ${(%):-%x})" != "_{{AppName}}" ]]; then
    compdef _{{AppName}} {{AppName}}
fi
