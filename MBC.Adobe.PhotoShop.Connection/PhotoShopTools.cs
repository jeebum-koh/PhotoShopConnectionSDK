/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// collection of tool id and name  in PhotoShop
    /// </summary>
    public static class PhotoShopTools
    {
        /// <summary>
        /// memory structure to hold tool id and name  in PhotoShop
        /// </summary>
        static Dictionary<string, string> _toolIDNameDictionary;

        /// <summary>
        /// populate tool id and name pairs
        /// </summary>
        static PhotoShopTools()
        {
            _toolIDNameDictionary =
                new Dictionary<string, string>()
                {
                    {"moveTool",                     "Move"},
                    {"marqueeRectTool",              "Marquee"},
                    {"marqueeEllipTool",             "Marquee Ellipse"},
                    {"marqueeSingleRowTool",         "Marquee Single Row"},
                    {"marqueeSingleColumnTool",      "Marquee Single Column"},
                    {"lassoTool",                    "Lasso"},
                    {"polySelTool",                  "Polygon Select"},
                    {"magneticLassoTool",            "Magnetic Lasso"},
                    {"quickSelectTool",              "Quick Select"},
                    {"magicWandTool",                "Magic Wand"},
                    {"cropTool",                     "Crop"},
                    {"sliceTool",                    "Slice"},
                    {"sliceSelectTool",              "Slice Select"},
                    {"spotHealingBrushTool",         "Spot Healing Brush"},
                    {"magicStampTool",               "Magic Stamp"},
                    {"patchSelection",               "Patch Selection"},
                    {"redEyeTool",                   "Red Eye"},
                    {"paintbrushTool",               "Paint Brush"},
                    {"pencilTool",                   "Pencil"},
                    {"colorReplacementBrushTool",    "Color Replacement Brush"},
                    {"cloneStampTool",               "Clone Stamp"},
                    {"patternStampTool",             "Pattern Stamp"},
                    {"historyBrushTool",             "History Brush"},
                    {"artBrushTool",                 "Art Brush"},
                    {"eraserTool",                   "Eraser"},
                    {"backgroundEraserTool",         "Background Eraser"},
                    {"magicEraserTool",              "Magic Eraser"},
                    {"gradientTool",                 "Gradient"},
                    {"bucketTool",                   "Bucket"},
                    {"blurTool",                     "Blur"},
                    {"sharpenTool",                  "Sharpen"},
                    {"smudgeTool",                   "Smudge"},
                    {"dodgeTool",                    "Dodge"},
                    {"burnInTool",                   "Burn"},
                    {"saturationTool",               "Saturation"},
                    {"penTool",                      "Pen"},
                    {"freeformPenTool",              "Freeform Pen"},
                    {"addKnotTool",                  "Add Knot"},
                    {"deleteKnotTool",               "Delete Knot"},
                    {"convertKnotTool",              "Convert Knot"},
                    {"typeCreateOrEditTool",         "Type Horizontal"},
                    {"typeVerticalCreateOrEditTool", "Type Vertical"},
                    {"typeCreateMaskTool",           "Type Horizontal Mask"},
                    {"typeVerticalCreateMaskTool",   "Type Vertical Mask"},
                    {"pathComponentSelectTool",      "Path Select"},
                    {"directSelectTool",             "Direct Select"},
                    {"rectangleTool",                "Rectangle"},
                    {"roundedRectangleTool",         "Rounded Rectangle"},
                    {"ellipseTool",                  "Ellipse"},
                    {"polygonTool",                  "Polygon"},
                    {"lineTool",                     "Line"},
                    {"customShapeTool",              "Custom Shape"},
                    {"textAnnotTool",                "Text Annotation"},
                    {"soundAnnotTool",               "Sound Annotation"},
                    {"eyedropperTool",               "Eye Dropper"},
                    {"colorSamplerTool",             "Color Sampler"},
                    {"rulerTool",                    "Ruler"},
                    {"countTool",                    "Count"},
                    {"set3DState",                   "3D"},
                    {"handTool",                     "Hand"},
                    {"zoomTool",                     "Zoom"},
                    {"wetBrushTool",                 "Mixer Brush"},
                    {"3DObjectRotateTool",           "3D Rotate"},
                    {"3DObjectRollTool",             "3D Roll"},
                    {"3DObjectPanTool",              "3D Pan"},
                    {"3DObjectSlideTool",            "3D Slide"},
                    {"3DObjectScaleTool",            "3D Scale"},
                    {"3DOrbitCameraTool",            "3D Rotate Camera"},
                    {"3DRollCameraTool",             "3D Roll Camera"},
                    {"3DPanCameraTool",              "3D Pan Camera"},
                    {"3DWalkCameraTool",             "3D Walk Camera"},
                    {"3DFOVTool",                    "3D Zoom"},
                    {"rotateTool",                   "Rotate"}
                };
        }

        /// <summary>
        /// check if given tool id is valid
        /// </summary>
        /// <param name="toolID">tool id to check</param>
        /// <returns>
        /// true, if given tool id is in our pre-defined collection.
        /// false, otherwise
        /// </returns>
        public static bool isValidToolID(
            string toolID)
        {
            return _toolIDNameDictionary.ContainsKey(toolID);
        }

        /// <summary>
        /// get tool name of given tool id
        /// </summary>
        /// <param name="toolID">tool id to resolve name</param>
        /// <returns>
        /// tool name if available,
        /// else "INVALID TOOL ID"
        /// </returns>
        public static string getToolName(
            string toolID)
        {
            return
                (isValidToolID(toolID)) ?
                _toolIDNameDictionary[toolID] :
                "INVALID TOOL ID";
        }

        /// <summary>
        /// tool id collection
        /// </summary>
        /// <returns>tool id collection</returns>
        public static IEnumerable<string> ToolIDs()
        {
            return _toolIDNameDictionary.Keys.AsEnumerable<string>();
        }

        /// <summary>
        /// tool name collection
        /// </summary>
        /// <returns>tool name collection</returns>
        public static IEnumerable<string> ToolNames()
        {
            return _toolIDNameDictionary.Values.AsEnumerable<string>();
        }

    }
}

















































































