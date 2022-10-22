##################################################################
#  First Steps in an Angular Client Web Project 9.0.0
#  to compile with an IM 9.1.0 API Server
#  -- LC: 2022-10-20, by HRA, OI TE
##################################################################


# --> Install APIServer with security (cert) but with anonymous authentication
#     (Default is Security + Windows authentication)

# --> Identity Manager Designer - Configuration Parameter
#  -> QBM\ApiServer\Defaults\SameSiteCookie -> none

# --> Clone the repository
git clone  git clone https://github.com/OneIdentity/IdentityManager.Imx.git ./identitymanager.imx-vid

# --> Install all Angular resources for this project
cd identitymanager.imx-vid/imxweb/
npm install

# --> Correct URL
#  -> EDIT: ...\imxweb\projects\qer-app-pwdportal\src\environments\environment.ts
#  -> Change URL to API-Server URL

# --> Build/start basic modules
npm run build:watch qbm
npm run build:watch qer

# --> Start the Web Portal
npm run start qer-app-portal

# --> In VSCODE
- Select "Debug"
- In the Run and Debug dropdown Select
  --> QER App Portal (Chrome)
- Hit the green RUN button

# --> Error 1: Server 500 or endless loading the login mask
#  -> Sign in with the Test-Browser to the ApiServer URL you entered above
   -> An API Server should return. If there are problems solve them first (in this Browser)

# --> Error 2: If you further run in an error Cross-site error
#  -> EDIT: ...\imxweb\projects\qbm\src\lib\api-client\api-client-fetc.ts
#  -> Add to imports
import { isDevMode } from '@angular/core';

#  -> Find and Add
 public async processRequest<T>(methodDescriptor: MethodDescriptor<T>): Promise<T> {
        const method = new MethodDefinition(methodDescriptor);
        const headers = new Headers(method.headers);
#  -> NEW content to ADD...
        if (isDevMode()) {
            headers.set("X-FORWARDED-PROTO", 'https');
        }
