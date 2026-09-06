using System.Collections.Generic;
using Godot;

namespace Empty.scripts;

public static class CellConstants
{
    public static Color EMPTY = new Color(110f / 255f, 123f / 255f, 130f / 255f);
    public static Color COLOR_TWO = new Color(238f / 255f, 228f / 255f, 218f / 255f);
    public static Color COLOR_FOUR = new Color(237f / 255f, 224f / 255f, 200f / 255f);
    public static Color COLOR_EIGHT = new Color(242f / 255f, 177f / 255f, 121f / 255f);
    public static Color COLOR_SIXTEEN = new Color(245f / 255f, 149f / 255f, 99f / 255f);
    public static Color COLOR_THIRTY_TWO = new Color(246f / 255f, 124f / 255f, 95f / 255f);
    public static Color COLOR_SIXTY_FOUR = new Color(246f / 255f, 94f / 255f, 59f / 255f);
    public static Color COLOR_HUNDRED_TWENTY_EIGHT = new Color(237f / 255f, 207f / 255f, 114f / 255f);
    public static Color COLOR_TWO_HUNDRED_FIFTY_SIX = new Color(237f / 255f, 204f / 255f, 97f / 255f);
    public static Color COLOR_FIVE_HUNDRED_TWELVE = new Color(237f / 255f, 200f / 255f, 80f / 255f);
    public static Color COLOR_ONE_THOUSAND_TWENTY_FOUR = new Color(237f / 255f, 197f / 255f, 63f / 255f);
    public static Color COLOR_TWO_THOUSAND_FORTY_EIGHT = new Color(237f / 255f, 194f / 255f, 46f / 255f);
    
    public static Dictionary<int, Color> NUMBERS_TO_COLORS = new ()
    {
        {2, COLOR_TWO},
        {4, COLOR_FOUR},
        {8, COLOR_EIGHT},
        {16, COLOR_SIXTEEN},
        {32, COLOR_THIRTY_TWO},
        {64, COLOR_SIXTY_FOUR},
        {128, COLOR_HUNDRED_TWENTY_EIGHT},
        {256, COLOR_TWO_HUNDRED_FIFTY_SIX},
        {512, COLOR_FIVE_HUNDRED_TWELVE},
        {1024, COLOR_ONE_THOUSAND_TWENTY_FOUR},
        {2048, COLOR_TWO_THOUSAND_FORTY_EIGHT},
    };
}