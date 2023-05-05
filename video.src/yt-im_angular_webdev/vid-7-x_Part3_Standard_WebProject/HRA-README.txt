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

# ************************ Video III.3 / #9 ************************

# --> Have a look at the  totorial-01 branch at IdentityM;anager.imx GitHub projecct
			NEW:
			- The branch
			- The SDK-samples folder in the project (Web-SDK)
			- The section with pre-released developer manuals in SDK-samples

# ************************ Video III.4 / #10 ************************

# --> Have a look at the  totorial-01 branch at IdentityM;anager.imx GitHub projecct
			--> All code samples/extensions we made are checked in there
 
# --> ------------------- Lesson 1... -------------------
      - Create an identity table using data-table and controls. 
			- Implement a detail side-sheet.
			
# --> Build QBM (once, without monitoring)
			npm run build qbm
			
# --> Build QER with whatchdog to rebuild on changes
			npm run build:watch qer
			
# --> Add a module to the qer library
			ng generate module sample-identity --project=qer
			
# --> Dry-run: Add a component to the newly created module in qer
			ng generate component sample-identity/sample-identities --project=qer --style=scss --export --prefix=ccc --skip-tests --dry-run

# --> Add a component to the newly created module in qer
			ng generate component sample-identity/sample-identities --project=qer --style=scss --export --prefix=ccc --skip-tests
			
# --> Add a route to the routing table to address the new component
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01
			
# --> Start the Portal for debugging purposes
			npm run start qer-app-portal
			
# --> Configure and start debugger -> defaul Google Chrome Bowser
			- Use [Run and Debug] and create a configuration for: "Web App (Chrome)"
			- Correct the port in parameter "url" (default is, 4200, but need to be the port your project uses)
			- Click the green run icon (left upper)

# --> Add a menu
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01
			

# ************************ Video III.5 / #11 ************************
# --> Continuing the exercise from video #10

# --> ------------------- ...Lesson 1... -------------------	 

# --> Add data table prerequisites
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01		
			
# --> Add data table html code
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01		 
			
# --> Add data table TS code
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01	

		
# ************************ Video III.6 / #12 ************************
# --> Continuing the exercise from video #11

# --> ------------------- ...Lesson 1 -------------------	

# --> Add a new component for the side-sheet
			ng generate component sample-identity/sample-identity-detailes --project=qer --style=scss --export --prefix=ccc --skip-tests

# --> Add the side-sheet prerequisites into the ts file
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01	
	
#	--> Create an eventhandler for the Details Button
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01	
			
#	--> Create an eventhandler for the Details Button
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01	
			
#	--> Add Details Button into the table
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01	
	
#	--> Add a tab-group into the side-sheet
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01			
			
#	--> Add data to the component to be used in trhe details
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01		
			
#	--> Add iEntity object and a CDR to fill the details side-sheet
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01			

#	--> Add HTML code to display the content of the side-sheet tabs
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01			

#	--> Add CSS code to improve the side-sheet layout
			code snippet see GitHub projecct: oneidentity/identitymanager.imx branch sample-01			
