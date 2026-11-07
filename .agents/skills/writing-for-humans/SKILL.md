---
name: writing-for-humans
description: Write plain prose without AI tells. Use when writing or editing text people read: replies, docs, READMEs, commit messages, PR and issue text, emails, chat posts.
disable-model-invocation: true
---

# Writing for humans

Write the way a careful person types: plain words, plain ASCII, the point first. Readers spot machine prose by how densely its tells cluster, so remove every one you find.

Two passes: write to **Write**, then run **The check** over the draft before it goes out. Done when the check finds nothing outside code, commands, quotations and names. Those, and typography the user's own text or language needs, stay as they are.

## Write

- **Lead with the point.** The first sentence is the answer or the change; the text ends when the content does.
- **Plain verbs**: is, has, uses, shows. "The cache is the bottleneck."
- **State the claim directly.** Mention the contrast only when someone actually believed the other thing.
- **As many items as there are.** Two or four is fine; reach for three only when there are three.
- **Concrete over general**: a number, a name, a file, a date. Name the source of a claim, or leave the claim out.
- **Neutral tone**: no enthusiasm, flattery or praise of the question; importance shown by the facts, not asserted.
- **Varied rhythm**: sentence and paragraph length follow the content.
- **Format for the destination.** Markdown only where it renders; plain text in commit messages, emails, chat posts and code comments. Headings in sentence case. Bold at most one term a section, and only one the reader must not miss. Lists for parallel items, sentences for reasoning.

## The check

### Characters

Search the draft for each, and replace:

| Find | Replace with |
|---|---|
| Em dash `—`, en dash `–` between words | A comma, colon, parentheses or a new sentence. A hyphen in ranges: `1-3`. |
| Curly quotes `“ ” ‘ ’` | Straight `"` and `'` |
| Ellipsis `…` | `...` |
| Symbols `→ ⇒ ≈ × ≥ ≤` | Words: "to", "about", "times", "at least". In technical text, ASCII: `->`, `x`, `>=`. |
| Emoji leading a heading or bullet (`✅ ❌ 🚀 💡 🔥`) | Nothing: the words carry the meaning |
| Invisible or unusual spaces (U+00A0, U+200B to U+200D, U+202F, U+2060, U+FEFF) | A normal space, or nothing |
| Markdown (`**bold**`, `###`, `- `) where it won't render | Plain text |

In a file, this lists every line that still holds one of them:

```sh
perl -CSD -ne 'print "$.: $_" if /[\x{2013}\x{2014}\x{2018}\x{2019}\x{201C}\x{201D}\x{2026}\x{2190}-\x{21FF}\x{2248}\x{00D7}\x{2264}\x{2265}\x{00A0}\x{200B}-\x{200D}\x{202F}\x{2060}\x{FEFF}\x{2600}-\x{27BF}\x{1F300}-\x{1FAFF}]/' FILE
```

### Words

Replace with the plain word, or cut:

| Find | Replace with |
|---|---|
| delve into, dive into | look at, cover |
| leverage, utilize | use |
| boasts, features (meaning "has") | has |
| serves as, stands as, acts as, functions as | is |
| underscore, highlight, showcase (as verbs) | show |
| foster, cultivate, bolster, enhance | build, help, improve, or the specific change |
| robust, seamless, comprehensive, holistic | what makes it so, or cut |
| crucial, pivotal, vital, key (as filler) | cut, or say why it matters |
| intricate, nuanced, multifaceted | name the parts |
| tapestry, landscape, realm, journey (abstract) | name the thing |
| testament to | shows |
| navigate (not about travel) | handle |
| meticulous, vibrant, rich, renowned, groundbreaking | cut |
| additionally, moreover, furthermore | also, or nothing |

### Phrases

Cut the whole phrase:

- **Openers**: "Great question!", "Certainly!", "Of course!", "Absolutely!", "You're absolutely right!", "I'd be happy to".
- **Closers**: "I hope this helps", "Let me know if you have any questions", "Feel free to", "In summary", "In conclusion", "Ultimately". A closing offer ("Would you like me to...?") stays only when the reader has a real choice to make.
- **Signposts**: "It's worth noting that", "It's important to note", "Let's dive in", "Let's break it down", "Here's the thing", "The key takeaway".
- **Inflated significance**: "plays a crucial role", "a testament to", "setting the stage for", "marks a significant shift", "in today's fast-paced world", "the ever-evolving landscape".
- **Vague attribution**: "experts say", "studies show", "many believe", "observers note", with no named source.

### Constructions

Rewrite:

- **Negative parallelism**: "It's not X, it's Y", "not just X, but Y", "not only X but also Y". State Y.
- **Participial tails**: ", highlighting its importance", ", ensuring a smooth experience", ", reflecting a broader trend". Cut it, or make it a sentence with a fact in it.
- **Triads**: three adjectives or three parallel phrases in a row. Keep the ones that are true.
- **Question then answer**: "The result? A faster build." Write "The build is faster."
- **Bold-label bullets** ("**Speed:** it is faster") where the label repeats the sentence. Write the sentence.
- **Stacked hedges**: "may potentially", "could possibly help to". One hedge, or none.
