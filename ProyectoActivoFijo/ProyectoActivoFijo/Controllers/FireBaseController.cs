using Firebase.Auth.Providers;
using Firebase.Auth; 
using Newtonsoft.Json; 
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
namespace ProyectoActivoFijo.Controllers
{

    public class FireBaseController
    {
        const string FB_PROJECT_ID = "dotnet-activos-fijos";
        const string FB_API_KEY = "AIzaSyDrPHhCEFEHDVhqRBXXGdPeWYaWdhwYIRI";
        const string FB_DOMAIN = "dotnet-activos-fijos.firebaseapp.com";

        const string FB_STORE_URL= $"https://firestore.googleapis.com/v1/projects/{FB_PROJECT_ID}/databases/(default)/documents";

   
        private readonly string apiKey = "TU_API_KEY";  // Si usas autenticación, puedes necesitar un token en su lugar
        private readonly HttpClient httpClient;


        FirebaseAuthClient clientFBAuth;

        FirebaseAuthConfig fbConfig = new FirebaseAuthConfig
        {
            ApiKey = FB_API_KEY,
            AuthDomain = FB_DOMAIN,
            Providers = new FirebaseAuthProvider[]
            {
                // Add and configure individual providers
                // new GoogleProvider().AddScopes("email"),
                new EmailProvider()
                // ...
            },
            //// WPF:
            //UserRepository = new FileUserRepository("FirebaseSample") // persist data into %AppData%\FirebaseSample
            //// UWP: 
            //UserRepository = new StorageRepository() // persist data into ApplicationDataContainer
        };

        public FireBaseController()
        {
            clientFBAuth = new FirebaseAuthClient(fbConfig);
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<UserCredential> SignInWithEmailAndPassowrd(string username, string password)
        {
            try
            {
              
                UserCredential userCredential = await clientFBAuth.SignInWithEmailAndPasswordAsync(username, password);
                return userCredential;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<T>> GetDataAsync<T>(string collection) where T : new()
        {
             
            // URL de la API REST de Firestore
            string urlFB = $"{FB_STORE_URL}/{collection}";

            // Solicitud GET a Firestore
            var response = await httpClient.GetAsync(urlFB);
            response.EnsureSuccessStatusCode();  // Lanza excepción si no es 200 OK

            var json = await response.Content.ReadAsStringAsync();
            List<T> dataList = DeserializarFirestoreResponse<T>(json);
            return dataList; 
        }

        public async Task<List<T>> GetDataAsync<T>(string collection, Dictionary<string, (string operador, object valor)> filtros) where T : new()
        {
            var url = $"{FB_STORE_URL}:runQuery";

            var filtrosLista = new List<object>();
            foreach (var filtro in filtros)
            {
                filtrosLista.Add(new
                {
                    fieldFilter = new
                    {
                        field = new { fieldPath = filtro.Key },
                        op = filtro.Value.operador,
                        value = CrearValorFirestore(filtro.Value.valor)
                    }
                });
            }

            var query = new
            {
                structuredQuery = new
                {
                    from = new[]
                    {
                    new { collectionId = collection }
                },
                    where = new
                    {
                        compositeFilter = new
                        {
                            op = "AND",
                            filters = filtrosLista
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(query);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener datos de {collection}");
            }
            var responseBody = await response.Content.ReadAsStringAsync();

            List<T> dataList = DeserializarFirestoreResponseFilter<T>(responseBody);
            return dataList;
        }

        private object CrearValorFirestore(object valor)
        {
            return valor switch
            {
                int i => new { integerValue = i },
                double d => new { doubleValue = d },
                bool b => new { booleanValue = b },
                string s => new { stringValue = s },
                DateTime dt => new { timestampValue = dt.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                _ => throw new ArgumentException("Tipo de dato no soportado.")
            };
        }

        public List<T> DeserializarFirestoreResponse<T>(string json) where T : new()

        { 
            var firestoreResponse = JsonConvert.DeserializeObject<FirestoreResponse<T>>(json);

            List<T> data = new List<T>();
             
            Dictionary<string, string> dicctionaryJsonProperty = GetDicctionaryJsonPropertyFromType(typeof(T));

            foreach (var doc in firestoreResponse.Documents)
            { 
                T itemData = new T(); 
                Type typeT = itemData.GetType();
                PropertyInfo[] propertiesOfType = typeT.GetProperties();

                Dictionary<string, FirestoreValue> jsonDocFields = doc.Fields;
                 
                foreach (var field in doc.Fields)
                {
                    string keyJsonDocument = field.Key;
                    var valueJsonDocument = field.Value;

                    // Busca en el diccionario JsonProperty

                    string keyFromDicctionary;
                    if (!dicctionaryJsonProperty.TryGetValue(keyJsonDocument, out string _))
                    {
                        continue;
                    }

                    keyFromDicctionary = dicctionaryJsonProperty[keyJsonDocument];
                    if (keyFromDicctionary == null)
                    {
                        continue;
                    }

                    // Se valida que exista en el modelo
                    PropertyInfo _propertyInfo = typeT.GetProperty(keyFromDicctionary);
                    if (_propertyInfo == null)
                    {
                        continue;
                    }

                    // Detecta y muestra el tipo de dato
                    if (valueJsonDocument.StringValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.StringValue);
                    else if (valueJsonDocument.ReferenceValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.ReferenceValue);
                    else if (valueJsonDocument.TimestampValue != null)
                        _propertyInfo.SetValue(itemData, DateTime.Parse(valueJsonDocument.TimestampValue, null, System.Globalization.DateTimeStyles.AssumeUniversal));
                    else if (valueJsonDocument.IntegerValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.IntegerValue);
                    else if (valueJsonDocument.DoubleValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.DoubleValue);
                    else if (valueJsonDocument.BooleanValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.BooleanValue);
                    else
                        throw new Exception($"No se encontro el tipo! {keyJsonDocument}-{keyFromDicctionary}");
                    //else if (valueJsonDocument.ArrayValue != null)

                }
               data.Add(itemData);
            }
            return data;
        }

        public List<T> DeserializarFirestoreResponseFilter<T>(string json) where T : new()

        {
            var firestoreResponse = JsonConvert.DeserializeObject<List<FirestoreResponseFilters<T>>>(json);

            List<T> data = new List<T>();

            Dictionary<string, string> dicctionaryJsonProperty = GetDicctionaryJsonPropertyFromType(typeof(T));

            foreach (var regFirestoreResponse in firestoreResponse)
            {
                var doc = regFirestoreResponse.Document;

                T itemData = new T();
                Type typeT = itemData.GetType();
                PropertyInfo[] propertiesOfType = typeT.GetProperties();

                Dictionary<string, FirestoreValue> jsonDocFields = doc.Fields;

                foreach (var field in doc.Fields)
                {
                    string keyJsonDocument = field.Key;
                    var valueJsonDocument = field.Value;

                    // Busca en el diccionario JsonProperty

                    string keyFromDicctionary;
                    if (!dicctionaryJsonProperty.TryGetValue(keyJsonDocument, out string _))
                    {
                        continue;
                    }

                    keyFromDicctionary = dicctionaryJsonProperty[keyJsonDocument];
                    if (keyFromDicctionary == null)
                    {
                        continue;
                    }

                    // Se valida que exista en el modelo
                    PropertyInfo _propertyInfo = typeT.GetProperty(keyFromDicctionary);
                    if (_propertyInfo == null)
                    {
                        continue;
                    }

                    // Detecta y muestra el tipo de dato
                    if (valueJsonDocument.StringValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.StringValue);
                    else if (valueJsonDocument.ReferenceValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.ReferenceValue);
                    else if (valueJsonDocument.TimestampValue != null)
                        _propertyInfo.SetValue(itemData, DateTime.Parse(valueJsonDocument.TimestampValue, null, System.Globalization.DateTimeStyles.AssumeUniversal));
                    else if (valueJsonDocument.IntegerValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.IntegerValue);
                    else if (valueJsonDocument.DoubleValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.DoubleValue);
                    else if (valueJsonDocument.BooleanValue != null)
                        _propertyInfo.SetValue(itemData, valueJsonDocument.BooleanValue);
                    else
                        throw new Exception($"No se encontro el tipo! {keyJsonDocument}-{keyFromDicctionary}");
                    //else if (valueJsonDocument.ArrayValue != null)

                }
                data.Add(itemData);
            }
            return data;
        }

        public Dictionary<string, string> GetDicctionaryJsonPropertyFromType(Type tipo)
        {
            Dictionary<string, string> diccionario = new Dictionary<string, string>();

            // Obtener las propiedades de la clase
            PropertyInfo[] propiedades = tipo.GetProperties();

            // Iterar sobre las propiedades y obtener los atributos JsonProperty
            foreach (var propiedad in propiedades)
            {
                var atributoJson = propiedad.GetCustomAttribute<JsonPropertyAttribute>();

                if (atributoJson != null)
                {
                    // Agregar al diccionario: nombre de la propiedad JSON y nombre de la propiedad de la clase
                    diccionario.Add(atributoJson.PropertyName, propiedad.Name);
                }
            }

            return diccionario;
        }



    }


}

public class FirestoreResponse<T>
{
    [JsonProperty("documents")]
    public List<Document> Documents { get; set; }
}

public class FirestoreResponseFilters<T>
{
    [JsonProperty("document")]
    public Document Document { get; set; }
}

public class Document
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("fields")]
    public Dictionary<string, FirestoreValue> Fields { get; set; }  // Dinámico

    [JsonProperty("createTime")]
    public string CreateTime { get; set; }

    [JsonProperty("updateTime")]
    public string UpdateTime { get; set; }
}

// Maneja cualquier tipo de valor en Firestore
public class FirestoreValue
{
    [JsonProperty("stringValue")]
    public string StringValue { get; set; }

    [JsonProperty("referenceValue")]
    public string ReferenceValue { get; set; }

    [JsonProperty("timestampValue")]
    public string TimestampValue { get; set; }

    [JsonProperty("integerValue")]
    public string IntegerValue { get; set; }

    [JsonProperty("doubleValue")]
    public double? DoubleValue { get; set; }

    [JsonProperty("booleanValue")]
    public bool? BooleanValue { get; set; }

    [JsonProperty("arrayValue")]
    public ArrayValue ArrayValue { get; set; }

    [JsonProperty("mapValue")]
    public MapValue MapValue { get; set; }
}

// Maneja valores tipo array
public class ArrayValue
{
    [JsonProperty("values")]
    public List<FirestoreValue> Values { get; set; }
}

// Maneja valores tipo mapa
public class MapValue
{
    [JsonProperty("fields")]
    public Dictionary<string, FirestoreValue> Fields { get; set; }
}
