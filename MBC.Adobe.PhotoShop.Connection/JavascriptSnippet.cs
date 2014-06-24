/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// collection of javascript snippet
    /// </summary>
    public static class JavascriptSnippet
    {
        /// <summary>
        /// collection of javascript snippets related with app object.
        /// </summary>
        public static class PhotoShopApp
        {
            /// <summary>
            /// version number, to test PS connectivity.
            /// </summary>
            public const string GET_VERSION = 
@"
app.version;
";

            /// <summary>
            /// comparison string used to check whether SET action succeeded.
            /// </summary>
            public const string ACTION_SUCCESS = 
"SUCCESS";

            /// <summary>
            /// documents length
            /// </summary>
            public const string GET_DOCUMENTS_LENGTH = 
@"
app.documents.length;
";

            /// <summary>
            /// active document id
            /// </summary>
            public const string GET_ACTIVE_DOCUMENT_ID =
@"
if (app.documents.length >0)
    app.activeDocument.id;
else
    '';
";

            /// <summary>
            /// get current foreground color of PhotoShop
            /// </summary>
            public static string GET_FOREGROUND_COLOR =
@"
app.foregroundColor.rgb.hexValue.toString();
";

            /// <summary>
            /// set foreground color.
            /// {0} - red byte value,
            /// {1} - green byte value,
            /// {2} - blue byte value,
            /// </summary>
            public static string SET_FOREGROUND_COLOR =
@"
var c = new SolidColor;
c.rgb.red =   {0};
c.rgb.green = {1};
c.rgb.blue =  {2};
app.foregroundColor = c;
'SUCCESS';
";

            /// <summary>
            /// get current background color of PhotoShop
            /// </summary>
            public static string GET_BACKGROUND_COLOR =
@"
app.backgroundColor.rgb.hexValue.toString();
";

            /// <summary>
            /// set background color.
            /// {0} - red byte value,
            /// {1} - green byte value,
            /// {2} - blue byte value,
            /// </summary>
            public static string SET_BACKGROUND_COLOR =
@"
var c = new SolidColor;
c.rgb.red =   {0};
c.rgb.green = {1};
c.rgb.blue =  {2};
app.backgroundColor = c;
'SUCCESS';
";

            /// <summary>
            /// get all document names
            /// </summary>
            public static string GET_ALL_DOCUMENT_NAMES =
@"
var docnames = '';
for (var i = 0; i < app.documents.length; i++)
{
    docnames += app.documents[i].name;
    if ((i + 1) != app.documents.length) 
    {
        docnames += String.fromCharCode(13);
    }
}
docnames;
";

            /// <summary>
            /// get active document name of PhotoShop.
            /// if there's no document opened in PhotoShop, null string
            /// </summary>
            public static string GET_ACTIVE_DOCUMENT_NAME =
@"
if (app.documents.length >0)
    app.activeDocument.Name;
else
    '';
";

            /// <summary>
            /// get current active tool id in PhotoShop
            /// </summary>
            public static string GET_ACTIVE_TOOL_ID =
@"
var ref = new ActionReference();
ref.putProperty( charIDToTypeID( 'Prpr' ), charIDToTypeID( 'Tool' ) );
ref.putEnumerated( charIDToTypeID( 'capp' ),  charIDToTypeID( 'Ordn' ), charIDToTypeID( 'Trgt' ) );
var desc = executeActionGet( ref );
var theTool = desc.getEnumerationType( charIDToTypeID( 'Tool' ) );
typeIDToStringID( theTool );
";

            /// <summary>
            /// set active tool.
            /// {0} - tool ID
            /// </summary>
            public static string SET_ACTIVE_TOOL_ID =
@"
try 
{
    var id93 = charIDToTypeID( 'slct' );
    var desc32 = new ActionDescriptor();
    var id94 = charIDToTypeID( 'null' );
    var ref30 = new ActionReference();
    ref30.putClass( stringIDToTypeID( '{0}' ) );
    desc32.putReference( id94, ref30 );
    executeAction( id93, desc32, DialogModes.NO );
    'SUCCESS';
}
catch(e) 
{
    'FAIL';
}
";

            /// <summary>
            /// load given file onto PS
            /// {0} - file path on host pc of PhotoShop
            /// </summary>
            public const string OPEN_FILE =
@"
app.open(new File('{0}'), OpenDocumentType.PHOTOSHOP);
'SUCCESS';
";

            /// <summary>
            /// save active document as...
            /// {0} - file name to be saved as on host pc of PhotoShop
            /// </summary>
            public const string SAVE_AS_ACTIVE_DOCUMENT =
@"
app.activeDocument.saveAs(new File('{0}'));
'SUCCESS';
";

            /// <summary>
            /// JPEG sucks... no idea to display returned jpeg from PS.
            /// turn to use PIXMAP.
            /// {0} - document ID, integer
            /// {1} - width of thumbnail, integer
            /// {2} - height of thumbnail, integer
            /// </summary>
            public const string GET_THUMBNAIL_IMAGE =
@"
var idNS = stringIDToTypeID( 'sendDocumentThumbnailToNetworkClient' );
var desc1 = new ActionDescriptor();
desc1.putInteger( stringIDToTypeID( 'documentID' ), {0});
desc1.putInteger( stringIDToTypeID( 'width' ),      {1});
desc1.putInteger( stringIDToTypeID( 'height' ),     {2});
desc1.putInteger( stringIDToTypeID( 'format' ),     2); 
executeAction( idNS, desc1, DialogModes.NO );
";
        }

        /// <summary>
        /// collection of javascript snippet which is involved with event notification.
        /// </summary>
        public static class PSEvent
        {
            /// <summary>
            /// reference javascript snippet to subscribe event.
            /// {0} - event ID
            /// {1} - SUCCESS string to check given script is well executed.
            /// </summary>
            public const string SUBSCRIBE_EVENT_REFERENCE =
@"
var idNS = stringIDToTypeID( 'networkEventSubscribe' );
var desc1 = new ActionDescriptor();
desc1.putClass( stringIDToTypeID( 'eventIDAttr' ), stringIDToTypeID( 'foregroundColorChanged' ) );
executeAction( idNS, desc1, DialogModes.NO );
'SUCCESS STR'
";

            /// <summary>
            /// used in SUBSCRIBE_EVENT javascript snippet generation
            /// </summary>
            public const string SUBSCRIBE_EVENT_HEADER =
@"
var idNS = stringIDToTypeID( 'networkEventSubscribe' );
";

            /// <summary>
            /// used in SUBSCRIBE_EVENT javascript snippet generation.
            /// {0} - identifier, should be unique
            /// {1} - event ID
            /// </summary>
            public const string SUBSCRIBE_EVENT_BODY = 
@"
var {0} = new ActionDescriptor();
{0}.putClass( stringIDToTypeID( 'eventIDAttr' ), stringIDToTypeID( '{1}' ) );
executeAction( idNS, {0}, DialogModes.NO );
";

            /// <summary>
            /// used in SUBSCRIBE_EVENT javascript snippet generation.
            /// </summary>
            public const string SUBSCRIBE_EVENT_FOOTER =
@"
'SUCCESS';
";
            /// <summary>
            /// comparison string used to check whether event subscription is handled well.
            /// </summary>
            public const string SUBSCRIBE_EVENT_SUCCESS = "SUCCESS";

        }

        /// <summary>
        /// collection of javascript snippets worth to check
        /// </summary>
        public static class REFERENCE
        {
            /// <summary>
            /// not working.....
            /// </summary>
            public const string GET_DOCUMENT_STREAM_NOT_WORKING =
@"
var idNS = stringIDToTypeID( 'sendDocumentStreamToNetworkClient );
var desc1 = new ActionDescriptor();
desc1.putInteger( stringIDToTypeID( 'documentID' ), {0} );
executeAction( idNS, desc1, DialogModes.NO );
";

            /// <summary>
            /// get environmental variable value of host pc of PhotoShop
            /// {0} - environmental variable name
            /// </summary>
            public const string GET_ENV_VAR =
@"
$.getenv('{0}')
";

            /// <summary>
            /// check given file existence on host pc of PhotoShop
            /// {0} - file name to check
            /// </summary>
            public const string FILE_EXISTS =
@"
var testFile = new File('{0}'); 
testFile.exists;
";

            /// <summary>
            /// check given folder existence on host pc of PhotoShop
            /// {0} - folder name to check
            /// </summary>
            public const string FOLDER_EXISTS =
@"
var testFolder = new Folder('{0}'); 
testFolder.exists;
";

            /// <summary>
            /// change text of given text layer.
            /// {0} - layer name,
            /// {1} - text to be changed
            /// </summary>
            public const string CHANGE_TEXT =
@"
app.activeDocument.layers.getByName('{0}').textItem.contents = '{2}';
'SUCCESS';
";

            /// <summary>
            /// change document file info's title text of active document.
            /// {0} - title text to be changed.
            /// </summary>
            public const string CHANGE_INFO_TITLE_ACTIVED_DOCUMENT =
@"
app.activeDocument.info.title = '{0}';
'SUCCESS';
";

            /// <summary>
            /// change document file info's title text of open document.
            /// {0} - document name
            /// {1} - title text to be changed.
            /// </summary>
            public const string CHANGE_INFO_TITLE_OPEN_DOCUMENT_BY_NAME =
@"
app.documents.getByName('{0}').info.title = '{1}';
'SUCCESS';
";

            /// <summary>
            /// change document file info's title text of open document.
            /// {0} - document id, integer
            /// {1} - title text to be changed.
            /// </summary>
            public const string CHANGE_INFO_TITLE_OPEN_DOCUMENT_BY_ID =
@"
app.documents[{0}].info.title = '{1}';
'SUCCESS';
";
        }
    }
}
