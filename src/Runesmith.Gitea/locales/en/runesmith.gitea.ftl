### The Gitea and Forgejo plugin's text in English, the source every translation starts from. The build turns each message into a
### member of the Strings class, such as Strings.SignInTo.
###
### In these messages $service is Gitea or Forgejo, $server is a server's address or name, such as gitea.com or Codeberg, and $account
### is the name of an account. They are never translated.

## Accounts, in Settings › Gitea and Settings › Forgejo.

add-account = Add Account
browser-sign-in-guide = How to set up browser sign-in
signed-in-browser = Signed in through the browser
signed-in-token = Signed in with an access token
# The state of an account whose sign-in ended.
signed-out-state = Signed out
manage-tokens = Manage Tokens
manage-tokens-tip = Open the applications settings on { $server }
sign-in = Sign In
sign-in-to = Sign in to { $service }
sign-out = Sign Out
remove-account-tip = Remove the account
not-signed-in = Not signed in
not-signed-in-hint = Sign in to clone from { $server }, push with your account and see pull requests. Add Account signs in to another server.
signed-in-as = Signed in to { $service } as { $account }
sign-out-title = Sign out of { $service }?
sign-out-message = Runesmith forgets the sign-in of { $account }. Cloning and pushing to { $server } then use your own Git setup.
# $account is the account's name, or the service's name when the account has none yet.
remove-account-title = Remove { $account }?
remove-account-message = Runesmith forgets the account on { $server } and its sign-in.
remove = Remove
signed-out-of = Signed out of { $service }
account-sign-in-refused = { $service } no longer accepts the sign-in of { $account }. Sign in again.
account-sign-in-expired = The { $service } sign-in of { $account } expired. Sign in again.
sign-in-needs-dialogs = Signing in needs a newer Runesmith
sign-in-needs-dialogs-message = This version of Runesmith does not let plugins show dialogs.
open-address-in-browser = Open this address in a browser
cancel = Cancel
back = Back
continue = Continue
try-again = Try Again

## The Add Account dialog.

add-account-title = Add a { $service } account
# $example is the address of a well-known server, such as gitea.com.
add-account-intro = Enter the address of a { $service } server, such as { $example } or a server of your own. Signing in comes next.
server-label = Server
add-account-hint = Add a port or a path when the server has one, such as git.example.com:3000 or example.com/git.
address-invalid = Enter a web address, such as git.example.com.
checking = Checking...
server-runs-forgejo = { $server } runs Forgejo. Add it in Settings › Forgejo.
server-runs-gitea = { $server } runs Gitea. Add it in Settings › Gitea.
server-not-found = No { $service } server answers at { $server }. Check the address.
probe-unreachable = { $server } could not be reached. Check the address and the connection.
probe-slow = { $server } took too long to answer. Check the address and try again.

## The sign-in dialog.

signing-in = Signing in...
browser-intro = Approve Runesmith on { $server } in your browser. Runesmith waits here for the answer and signs you in.
browser-waiting = Waiting for the browser...
browser-open-again = Open the page again
browser-hint = The browser asks you to sign in to { $server } first if you are not. Nothing you type there reaches Runesmith.
browser-failed = Signing in through the browser did not work. Try again, or use an access token from { $server }.
use-token-instead = Use a token instead
use-browser = Use the Browser
token-intro = Paste an access token from your settings on { $server }. Runesmith checks it with the server before keeping it.
token-placeholder = Access token
token-scopes = Give the token these permissions:
# What each scope of a token needs, shown after the scope's name: repository, user and organization.
token-scope-repository = Read and write, to clone, push, and list and create pull requests
token-scope-user = Read, for your login and avatar, and your repositories
token-scope-organization = Read, for the organizations whose repositories you clone
token-create = Create a token on { $server }
token-refused = { $server } does not accept this token. Check that it is complete and not deleted.
token-forbidden = The token may not read your account. Give it the user scope with Read access.
tokens-in-system-store = Tokens are kept in the system's secret store.
tokens-in-file = No system secret store was found, so tokens are kept in a file only you can read.
tokens-in-memory = Tokens are kept only until Runesmith closes.

# Setting up sign-in through the browser. The names in bold are the server's own; they are filled in as they are.
setup-offer = To sign in through the browser instead, register an OAuth2 application for Runesmith on { $server } once.
setup-link = Set up browser sign-in
setup-intro = Register Runesmith as an OAuth2 application on { $server }, once. Signing in then takes a click in the browser.
# $page is Settings › Applications and $section is Manage OAuth2 applications.
setup-step-open = Open { $page } on { $server }, under { $section }.
# $name is Runesmith, $redirect is http://127.0.0.1/ and $option is Confidential client.
setup-step-name = Name it { $name }, set the redirect URI to { $redirect } and clear { $option }.
# $button is Create application and $field is Client ID.
setup-step-create = Select { $button } and paste its { $field } here. Runesmith needs no client secret.
setup-open-applications = Open { $server } Applications
setup-save = Save and Sign In

## Settings.

setting-client-ids = OAuth2 client IDs
# $example is the address of the service's best-known server, and $server its name.
setting-client-ids-description = The client ID of the OAuth2 application registered for Runesmith on each server, as server=ID pairs separated by commas, such as { $example }=1a2b3c4d. An ID without a server is for { $server }. Servers without one sign in with an access token.

## What a server answers.

sign-in-to-continue = Sign in to { $server } to continue.
server-unreachable = { $server } could not be reached. Check the connection and try again.
server-slow = { $server } took too long to answer. Try again in a moment.
server-bad-answer = { $server } sent an answer Runesmith does not understand.
server-empty-answer = { $server } sent an empty answer.
sign-in-refused = { $server } no longer accepts this sign-in. Sign in to { $server } again.
sign-in-expired = The { $server } sign-in expired. Sign in to { $server } again.
# $reason is the server's own message.
refused-reason = { $server } refused: { $reason }. Check that the token has the scopes Runesmith needs.
refused = { $server } refused. Check that the token has the scopes Runesmith needs.
not-found = { $server } could not find it, or the account has no access to it.
not-accepted-reason = { $server } did not accept it: { $reason }.
not-accepted = { $server } did not accept the request.
# Variables:
#   $status (Number) - the HTTP status code
server-error = { $server } answered with an error ({ $status }). Try again in a moment.

## Signing in through the browser.

oauth-not-finished = { $server } did not finish the sign-in. Try again.
oauth-not-finished-reason = { $server } did not finish the sign-in: { $reason }.
oauth-not-finished-reason-retry = { $server } did not finish the sign-in: { $reason }. Try again.
# $redirect is the address the application must send the browser back to.
oauth-unknown-client = { $server } does not know the OAuth2 application in the client ID setting, or its redirect URI is not { $redirect }.
oauth-refused-client = { $server } refused the OAuth2 application: check its client ID and that its redirect URI is { $redirect }.
oauth-server-failed = { $server } could not finish the sign-in. Try again in a moment.
oauth-denied = The sign-in was cancelled on { $server }.
oauth-no-port = Runesmith could not open a local port for the browser's answer.
# The page the browser shows when the sign-in is done: its title and its text.
page-signed-in = Signed in
page-signed-in-message = Runesmith received the approval. You can close this tab and go back to Runesmith.
page-not-approved-message = Runesmith was not approved. You can close this tab.
page-wrong-sign-in = Not this sign-in
page-wrong-sign-in-message = This answer belongs to another sign-in. Use the newest browser tab Runesmith opened.
page-not-found = Not found
page-not-found-message = This address is only for signing in.
