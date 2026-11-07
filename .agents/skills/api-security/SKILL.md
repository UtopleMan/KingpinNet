---
name: api-security
description: Securing an API. Use when designing or reviewing authentication, authorization, tokens, sessions or login, or protecting an API with rate limiting, CORS, input handling and network controls.
---

# API Security

Every API answers three questions on each request, in this order: **who is calling** (authentication), **what may they do** (authorization), and **is this request safe to process** (hardening). Design all three for every endpoint, and check each one in the order below.

## 1. Authentication: who is calling

| Mechanism | Use |
|---|---|
| **Basic** (Base64 of username:password, trivially reversible) | Internal tools only, and only over HTTPS |
| **Bearer token** in the `Authorization` header | The default for APIs: stateless, so it scales |
| **OAuth 2.0 + OpenID Connect** | Login through an identity provider (Google, GitHub, Entra ID). The provider issues a signed **ID token** (a JWT) saying who the user is. |
| **SSO** over OIDC or SAML | One login across several services; SAML (XML) mainly where enterprise and legacy systems require it |

Tokens:

- A **JWT** is signed, not encrypted: anyone can read it, so it holds claims (`sub`, roles, scopes, `exp`, `iss`, `aud`), never secrets. The API validates the signature, `exp`, `iss` and `aud` on every request.
- **Short-lived access tokens** (minutes) authorize API calls. **Long-lived refresh tokens** only fetch new access tokens, so users stay logged in while a leaked access token soon expires.
- Refresh tokens stay out of reach of page scripts: in an `HttpOnly`, `Secure`, `SameSite` cookie, or held server-side by a backend-for-frontend. Rotate them on every use and revoke them on logout.

A failed authentication returns `401`.

## 2. Authorization: what they may do

Authentication tells you who; authorization decides what, on every request, at the server. Pick the model that fits the access rules, and combine them where the rules need it:

- **RBAC** (role-based): users get roles (admin, editor, viewer), roles get permissions. The default: simple to reason about and audit. Fits dashboards, CMS tools, team management.
- **ABAC** (attribute-based): a policy decides from user attributes (department), resource attributes (owner, classification) and environment (time, location, device). Fits rules roles cannot express, like "HR may read internal HR documents during office hours". More flexible, but policies need managing and can conflict.
- **ACL** (access control list): each resource lists which users have which permissions on it, as a shared document does. Fits per-object sharing; at millions of users and objects it needs careful storage and indexing.

A token carries **identity and claims** (user, roles, scopes); the authorization model is the **rule** applied to them. Delegated access, where a third-party app acts for a user, goes through OAuth 2.0: the user approves specific **scopes**, the app gets a token limited to them and never sees the user's password.

A failed authorization returns `403`; return `404` instead when admitting the resource exists would leak information.

## 3. Hardening: is this request safe

- **Rate limiting** per user or API key, per IP, per endpoint (tighter on login and expensive endpoints), plus a global ceiling so a botnet that stays under each per-client limit still cannot flood the system. Over the limit: `429` with `Retry-After`.
- **CORS**: allow only the exact origins of your own front ends. Never `*` on an API that uses cookies or credentials.
- **Injection**: user input reaches a SQL or NoSQL query only as a parameter (parameterized queries, the ORM's query API), never by string concatenation.
- **Input validation**: check type, length, range and format at the edge, and reject what fails with `400` or `422`.
- **XSS**: store user content as data and encode it for its context on output (HTML-escape before rendering); a `Content-Security-Policy` limits what an injected script could do.
- **CSRF**: an API authenticated by cookies needs `SameSite` cookies plus an anti-forgery token on state-changing requests. Bearer tokens in headers are not sent automatically, so they are not exposed to CSRF.
- **Firewall**: a WAF in front of public APIs filters known attack patterns before they reach the code.
- **Network**: internal and admin APIs are reachable only from the private network (VPN, private subnet), not from the internet.
- **Transport**: HTTPS everywhere, HSTS on.

Done when every endpoint names its authentication, its authorization rule and its rate limit, and every hardening item above is either in place or recorded as not applying, with the reason.
