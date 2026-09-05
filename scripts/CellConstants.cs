using System.Collections.Generic;
using Godot;

namespace Empty.scripts;

public static class CellConstants
{
    public static Color EMPTY = new Color(110f / 255f, 123f / 255f, 130f / 255f);
    public static Color COLOR_TWO = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_FOUR = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_EIGHT = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_SIXTEEN = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_THIRTY_TWO = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_SIXTY_FOUR = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_HUNDRED_TWENTY_EIGHT = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_TWO_HUNDRED_FIFTY_SIX = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_FIVE_HUNDRED_TWELVE = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_ONE_THOUSAND_TWENTY_FOUR = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public static Color COLOR_TWO_THOUSAND_FORTY_EIGHT = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    
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