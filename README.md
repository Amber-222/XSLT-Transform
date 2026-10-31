# Sales Order & Acknowledgment Integration

A split-solution ASP.NET Core architecture designed to process raw Sales Order JSON files, apply XSLT transformations, generate standardized output files with dynamic naming conventions, convert files to XML, and securely handle local file drops and downloads between an API backend and an MVC frontend client.

## 🚀 Tech Stack

### 1. Backend API (`XSLT_Transform`)

| Component              | Technology / Approach                                      |
|------------------------|------------------------------------------------------------|
| Framework              | ASP.NET Core Web API (.NET)                                |
| Processing Engine      | Custom `TransformService` leveraging XML/XSLT transformations |
| Data Formatting        | Newtonsoft.Json (`JObject`) for dynamic parsing and property mapping |
| File Management        | Static Files Middleware (`app.UseStaticFiles()`) serving generated assets from `wwwroot/processed` |
| Security / Config      | CORS policy (`AllowAll`) supporting cross-origin communication and header exposure |

### 2. Frontend Client (`SalesOrderProcessing`)

| Component              | Technology / Approach                                      |
|------------------------|------------------------------------------------------------|
| Framework              | ASP.NET Core MVC                                           |
| UI Structure           | Razor Pages (`Index.cshtml`) styled with Bootstrap components and forms |
| Network & Proxy        | `HttpClient` service handling multipart form-data transmissions and a proxy download handler (`DownloadFromApi`) to securely stream files across local ports |

## 💡 Core Logic & Workflow Architecture

### 1. Upload & Submission
The user uploads a raw JSON file (`SO.json`) through the MVC frontend interface (`OrderController`).

### 2. XSLT Transformation & Dynamic Naming
The API backend endpoint `/api/SalesOrders/process` receives the file, extracts metadata using `TransformService`, and transforms the input via specific XSLT stylesheets:

- `SalesOrderTransform.xslt`
- `SalesOrderAckTransform.xslt`

**Output files are dynamically named following strict company conventions:**

| File Type       | Naming Convention                          |
|-----------------|--------------------------------------------|
| Sales Order     | `{OrderRef}-{CategoryCode}-{Date}.json`    |
| Acknowledgment  | `{OrderRef}-{InternalRef}-{Date}.json`     |

### 3. Server-Side File Drop
The API physically saves both generated `.json` files into its `wwwroot/processed` directory.  
It constructs public static URLs and returns them in a JSON payload.

### 4. MVC Proxy Download
Instead of direct browser-to-API calls (which can trigger local CORS/port/SSL blocks), the MVC frontend passes the file URL through its own `DownloadFromApi` controller proxy, streaming the file down to the user’s browser with the exact dynamic filename.

### 5. XML Conversion
Additional endpoints (`convert-so-to-xml` and `convert-ack-to-xml`) allow users to upload expected JSON outputs and instantly download their converted XML equivalents.

## 📱 How the System Works
Upload a raw Sales Order JSON file (SO.json) through the MVC frontend
The API receives the file and applies XSLT transformations
Two standardized JSON files are generated with dynamic names
Files are saved to the API’s wwwroot/processed folder
The MVC frontend downloads both files via a secure proxy
Optionally convert the generated JSON files to XML

## Main Features
 📤 Process Sales Order JSON
 🔄 XSLT Transformation (Sales Order + Acknowledgment)
 📁 Dynamic file naming
 💾 Server-side file drop (wwwroot/processed)
 ⬇️ Secure proxy downloads
 📄 Convert Sales Order JSON → XML
 📄 Convert Acknowledgment JSON → XML

## 🛠 Tech StackBackend API (XSLT_Transform)
 Framework: ASP.NET Core Web API (.NET)
 Processing Engine: Custom TransformService (XML/XSLT)
 Data Formatting: Newtonsoft.Json (JObject)
 File Management: Static Files Middleware (app.UseStaticFiles())
 Security: CORS policy (AllowAll)

## Frontend Client (SalesOrderProcessing)
 Framework: ASP.NET Core MVC
 UI: Razor Pages + Bootstrap
 Networking: HttpClient + multipart form-data
 Proxy Download: DownloadFromApi controller