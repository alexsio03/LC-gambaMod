using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine;

namespace gamba.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        private static char[,] symbols =
        {
            {'O', 'X', '#', '@'},
            {'*', '?', '$', '&'}
        };

        private static System.Random random = new System.Random();

        private static string[,] GetSlots()
        {
            string[,] slots = new string[5, 5];

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    slots[row, col] = $"{symbols[random.Next(0, symbols.GetLength(0)), random.Next(0, symbols.GetLength(0))]}";
                }
            }

            return slots;
        }

        private static string[,] SpinSlots(string[,] slots)
        {
            for (int row = 0; row < 5; row++)
            {
                for(int col = 0; col < 5; col++)
                {
                    slots[row, col] = $"{symbols[random.Next(0, 1), random.Next(0, 4)]}";
                }
            }
            return slots;
        }

        private static int CalculateWinnings(string[,] slots, int gambleAmount)
        {
            int winnings = 0;

            // Check for a win in each row
            for (int row = 0; row < 5; row++)
            {
                if (CheckLine(slots[row, 0], slots[row, 1], slots[row, 2], slots[row, 3], slots[row, 4]))
                {
                    winnings += gambleAmount * 2; // Adjust the multiplier as needed
                }
            }

            // Check for a win in each column
            for (int col = 0; col < 5; col++)
            {
                if (CheckLine(slots[0, col], slots[1, col], slots[2, col], slots[3, col], slots[4, col]))
                {
                    winnings += gambleAmount * 2; // Adjust the multiplier as needed
                }
            }

            // Check for a win in the main diagonal (top-left to bottom-right)
            if (CheckLine(slots[0, 0], slots[1, 1], slots[2, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            // Check for a win in the secondary diagonal (top-right to bottom-left)
            if (CheckLine(slots[0, 4], slots[1, 3], slots[2, 2], slots[3, 1], slots[4, 0]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            // Check for a win in the secondary diagonal (top-right to bottom-left)
            if (CheckLine(slots[0, 4], slots[1, 3], slots[2, 2], slots[3, 1], slots[4, 0]) && CheckLine(slots[0, 0], slots[1, 1], slots[2, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += gambleAmount * 10; // Adjust the multiplier as needed
            }

            if (CheckLine(slots[0, 0], slots[1, 1], slots[2, 2], slots[1, 3], slots[0, 4]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            if (CheckLine(slots[0, 0], slots[1, 1], slots[1, 2], slots[1, 3], slots[0, 4]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            if (CheckLine(slots[4, 0], slots[3, 1], slots[2, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            if (CheckLine(slots[4, 0], slots[3, 1], slots[3, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            if (CheckLine(slots[2, 0], slots[1, 1], slots[0, 2], slots[1, 3], slots[2, 4]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            if (CheckLine(slots[2, 0], slots[3, 1], slots[4, 2], slots[3, 3], slots[2, 4]))
            {
                winnings += gambleAmount * 3; // Adjust the multiplier as needed
            }

            return winnings;
        }

        // Helper method to check if all elements in a line are the same
        private static bool CheckLine(params string[] symbols)
        {
            return symbols.All(symbol => symbol == symbols[0]);
        }

        private static string updateScreen(string[,] slots)
        {
            return "Welcome to the Company slot machine\n" +
                "---------------------------------------------------\n" +
                "\n\n" +
                $"         {slots[0, 0]}   |   {slots[0, 1]}   |   {slots[0, 2]}   |   {slots[0, 3]}   |   {slots[0, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[1, 0]}   |   {slots[1, 1]}   |   {slots[1, 2]}   |   {slots[1, 3]}   |   {slots[1, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[2, 0]}   |   {slots[2, 1]}   |   {slots[2, 2]}   |   {slots[2, 3]}   |   {slots[2, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[3, 0]}   |   {slots[3, 1]}   |   {slots[3, 2]}   |   {slots[3, 3]}   |   {slots[3, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[4, 0]}   |   {slots[4, 1]}   |   {slots[4, 2]}   |   {slots[4, 3]}   |   {slots[4, 4]}         \n" +
                "\n\n" +
                "---------------------------------------------------\n" +
                "Spinning....\n";
        }

        private static string updateSlotScreen(string[,] slots)
        {
            return "Welcome to the Company slot machine\n" +
                "---------------------------------------------------\n" +
                "\n\n" +
                $"         {slots[0, 0]}   |   {slots[0, 1]}   |   {slots[0, 2]}   |   {slots[0, 3]}   |   {slots[0, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[1, 0]}   |   {slots[1, 1]}   |   {slots[1, 2]}   |   {slots[1, 3]}   |   {slots[1, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[2, 0]}   |   {slots[2, 1]}   |   {slots[2, 2]}   |   {slots[2, 3]}   |   {slots[2, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[3, 0]}   |   {slots[3, 1]}   |   {slots[3, 2]}   |   {slots[3, 3]}   |   {slots[3, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[4, 0]}   |   {slots[4, 1]}   |   {slots[4, 2]}   |   {slots[4, 3]}   |   {slots[4, 4]}         \n" +
                "\n\n" +
                "---------------------------------------------------\n" +
                "Please enter how much you would like to gamble\n(bet [amount]): ";    
        }

        private static string winScreen(string[,] slots, int winnings)
        {
            return "Welcome to the Company slot machine\n" +
                "---------------------------------------------------\n" +
                "\n\n" +
                $"         {slots[0, 0]}   |   {slots[0, 1]}   |   {slots[0, 2]}   |   {slots[0, 3]}   |   {slots[0, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[1, 0]}   |   {slots[1, 1]}   |   {slots[1, 2]}   |   {slots[1, 3]}   |   {slots[1, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[2, 0]}   |   {slots[2, 1]}   |   {slots[2, 2]}   |   {slots[2, 3]}   |   {slots[2, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[3, 0]}   |   {slots[3, 1]}   |   {slots[3, 2]}   |   {slots[3, 3]}   |   {slots[3, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[4, 0]}   |   {slots[4, 1]}   |   {slots[4, 2]}   |   {slots[4, 3]}   |   {slots[4, 4]}         \n" +
                "\n\n" +
                "---------------------------------------------------\n" +
                $"Won {winnings} credits!\n";
        }

        static string[,] slots = GetSlots();
        static TerminalNode spinNode = new TerminalNode();
        static TerminalNode savedNode = new TerminalNode();
        static int betAmount = 0;

        private static bool TryParseBetAmount(string input, out int betAmount)
        {
            // Split the input into parts
            string[] parts = input.Split(' ');

            // Check if the input has the correct format
            if (parts.Length == 2 && int.TryParse(parts[1], out betAmount))
            {
                return true;
            }

            // If the format is incorrect or parsing fails, set betAmount to zero
            betAmount = 0;
            return false;
        }

        [HarmonyPatch("ParsePlayerSentence")]
        [HarmonyPrefix]
        static void stringCheck(ref Terminal __instance)
        {
            string s = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (s.StartsWith("bet"))
            {
                if (TryParseBetAmount(s, out betAmount))
                {
                    savedNode = __instance.terminalNodes.specialNodes[11];
                    __instance.terminalNodes.specialNodes[11] = spinNode;
                }
            }
        }

        [HarmonyPatch("LoadNewNode")]
        [HarmonyPrefix]
        static void spinGamba(ref TerminalNode node, ref Terminal __instance)
        {
            if (node.terminalEvent == "spinGamba")
            {
                __instance.terminalNodes.specialNodes[11] = savedNode;
                if (__instance.groupCredits - betAmount >= 0)
                {
                    __instance.groupCredits -= betAmount;
                    node.displayText = updateScreen(SpinSlots(slots));
                    int winnings = CalculateWinnings(slots, betAmount);
                    if (winnings > 0)
                    {
                        node.displayText = winScreen(slots, winnings);
                        __instance.PlayTerminalAudioServerRpc(0);
                        __instance.PlayTerminalAudioServerRpc(3);
                    }
                    else
                    {
                        __instance.PlayTerminalAudioServerRpc(1);
                    }
                    __instance.groupCredits += winnings;
                }
                
            }
            if (node.terminalEvent == "gamba")
            {
                node.displayText = updateSlotScreen(SpinSlots(slots));
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void addGamba(ref Terminal __instance)
        {
            TerminalNode gambaNode = new TerminalNode();
            gambaNode.displayText = "Welcome to the Company slot machine\n" +
                "---------------------------------------------------\n" +
                "\n\n" +
                $"         {slots[0, 0]}   |   {slots[0, 1]}   |   {slots[0, 2]}   |   {slots[0, 3]}   |   {slots[0, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[1, 0]}   |   {slots[1, 1]}   |   {slots[1, 2]}   |   {slots[1, 3]}   |   {slots[1, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[2, 0]}   |   {slots[2, 1]}   |   {slots[2, 2]}   |   {slots[2, 3]}   |   {slots[2, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[3, 0]}   |   {slots[3, 1]}   |   {slots[3, 2]}   |   {slots[3, 3]}   |   {slots[3, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[4, 0]}   |   {slots[4, 1]}   |   {slots[4, 2]}   |   {slots[4, 3]}   |   {slots[4, 4]}         \n" +
                "\n\n" +
                "---------------------------------------------------\n" +
                "Please enter how much you would like to gamble\n(bet [amount]): ";
            gambaNode.clearPreviousText = true;
            gambaNode.terminalEvent = "gamba";

            TerminalKeyword gambaKey = new TerminalKeyword();
            gambaKey.word = "slots";
            gambaKey.isVerb = false;
            gambaKey.specialKeywordResult = gambaNode;



            slots = SpinSlots(slots);
            spinNode.displayText = "Welcome to the Company slot machine\n" +
                "---------------------------------------------------\n" +
                "\n\n" +
                $"         {slots[0, 0]}   |   {slots[0, 1]}   |   {slots[0, 2]}   |   {slots[0, 3]}   |   {slots[0, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[1, 0]}   |   {slots[1, 1]}   |   {slots[1, 2]}   |   {slots[1, 3]}   |   {slots[1, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[2, 0]}   |   {slots[2, 1]}   |   {slots[2, 2]}   |   {slots[2, 3]}   |   {slots[2, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[3, 0]}   |   {slots[3, 1]}   |   {slots[3, 2]}   |   {slots[3, 3]}   |   {slots[3, 4]}         \n" +
                "             |       |       |       |             \n" +
                $"         {slots[4, 0]}   |   {slots[4, 1]}   |   {slots[4, 2]}   |   {slots[4, 3]}   |   {slots[4, 4]}         \n" +
                "\n\n" +
                "---------------------------------------------------\n" +
                "Spinning....\n";
            spinNode.clearPreviousText = true;
            spinNode.terminalEvent = "spinGamba";

            TerminalKeyword spinKey = new TerminalKeyword();
            spinKey.word = "bet";
            spinKey.isVerb = false;
            spinKey.specialKeywordResult = spinNode;



            __instance.terminalNodes.allKeywords = __instance.terminalNodes.allKeywords.AddToArray(gambaKey);
            __instance.terminalNodes.allKeywords = __instance.terminalNodes.allKeywords.AddToArray(spinKey);

            foreach (TerminalKeyword keyword in __instance.terminalNodes.allKeywords)
            {
                Console.WriteLine(keyword.word);
                if (keyword.word.Equals("store"))
                {
                    Console.WriteLine(keyword.specialKeywordResult.displayText);
                    Console.WriteLine(keyword.specialKeywordResult.terminalEvent);
                    Console.WriteLine(keyword.specialKeywordResult.terminalOptions[0].noun.word);
                }
                if (keyword.word.Equals("other"))
                {
                    int ind = keyword.specialKeywordResult.displayText.IndexOf("t.");
                    string word = keyword.specialKeywordResult.displayText;
                    keyword.specialKeywordResult.displayText = word.Substring(0, ind + 2) + "\n\n>SLOTS\nGamble company credits in the slot machine.\n";
                    Console.WriteLine(keyword.specialKeywordResult.displayText);
                }
            }
        }
    }
}
