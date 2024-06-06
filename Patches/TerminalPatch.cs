using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace gamba.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        private static char[,] symbols =
        {
            {'O', 'X', '#'},
            {'*', '?', '@'}
        };

        private static System.Random random = new System.Random();

        private static string[,] GetSlots()
        {
            string[,] slots = new string[5, 5];

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    slots[row, col] = $"{GetRandomSymbol(row, col)}";
                }
            }

            return slots;
        }

        private static string[,] SpinSlots(string[,] slots)
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    // Adjust the probability of symbols based on their indices
                    slots[row, col] = $"{GetRandomSymbol(row, col)}";
                }
            }
            return slots;
        }

        private static char GetRandomSymbol(int row, int col)
        {
            int probability = random.Next(1, 101); // Generates a random number between 1 and 100

            // Define the probability ranges for each symbol based on their indices
            int commonProbability = 40;  // Adjust as needed
            int uncommon1Probability = 25;  // Adjust as needed
            int uncommon2Probability = 20;  // Adjust as needed
            int rare1Probability = 5;  // Adjust as needed
            int rare2Probability = 3;  // Adjust as needed

            if (probability <= commonProbability)
            {
                return symbols[0, 0];  // Most common symbol at index [0, 0]
            }
            else if (probability <= commonProbability + uncommon1Probability)
            {
                return symbols[0, 1];  // Less common symbol at index [0, 1]
            }
            else if (probability <= commonProbability + uncommon1Probability + uncommon2Probability)
            {
                return symbols[0, 2];  // Another less common symbol at index [0, 2]
            }
            else if (probability <= commonProbability + uncommon1Probability + uncommon2Probability + rare1Probability)
            {
                return symbols[1, 0];  // Rare symbol at index [1, 0]
            }
            else if (probability <= commonProbability + uncommon1Probability + uncommon2Probability + rare1Probability + rare2Probability)
            {
                return symbols[1, 1];  // Another rare symbol at index [1, 1]
            }
            else
            {
                return symbols[1, 2];  // Rarest symbol at index [1, 2]
            }
        }

        private static int CalculateWinnings(string[,] slots, int gambleAmount)
        {
            int winnings = 0;

            Dictionary<string, int> symbolMultipliers = new Dictionary<string, int>
            {
                {"O", 2},  // Adjust the multiplier for symbol 'O'
                {"X", 3},  // Adjust the multiplier for symbol 'X'
                {"#", 4},  // Adjust the multiplier for symbol '#'
                {"*", 8},  // Adjust the multiplier for symbol '*'
                {"?", 10},  // Adjust the multiplier for symbol '?'
                {"@", 20}  // Adjust the multiplier for symbol '@' (the rarest)
            };

            // Check for a win in each row
            for (int row = 0; row < 5; row++)
            {
                if (CheckLine(slots[row, 0], slots[row, 1], slots[row, 2], slots[row, 3], slots[row, 4]))
                {
                    winnings += CalculateLineWinnings(slots[row, 0], symbolMultipliers, gambleAmount);
                }
            }

            // Check for a win in each column
            for (int col = 0; col < 5; col++)
            {
                if (CheckLine(slots[0, col], slots[1, col], slots[2, col], slots[3, col], slots[4, col]))
                {
                    winnings += CalculateLineWinnings(slots[0, col], symbolMultipliers, gambleAmount);
                }
            }

            // Check for a win in the main diagonal (top-left to bottom-right)
            if (CheckLine(slots[0, 0], slots[1, 1], slots[2, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += CalculateLineWinnings(slots[0, 0], symbolMultipliers, gambleAmount);
            }

            // Check for a win in the secondary diagonal (top-right to bottom-left)
            if (CheckLine(slots[0, 4], slots[1, 3], slots[2, 2], slots[3, 1], slots[4, 0]))
            {
                winnings += CalculateLineWinnings(slots[0, 4], symbolMultipliers, gambleAmount);
            }

            // Check for a win in the secondary diagonal (top-right to bottom-left)
            if (CheckLine(slots[0, 4], slots[1, 3], slots[2, 2], slots[3, 1], slots[4, 0]) && CheckLine(slots[0, 0], slots[1, 1], slots[2, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += CalculateLineWinnings(slots[0, 4], symbolMultipliers, gambleAmount);
            }

            // Top-mid V line
            if (CheckLine(slots[0, 0], slots[1, 1], slots[2, 2], slots[1, 3], slots[0, 4]))
            {
                winnings += CalculateLineWinnings(slots[0, 0], symbolMultipliers, gambleAmount);
            }

            // Top-trapezoid line
            if (CheckLine(slots[0, 0], slots[1, 1], slots[1, 2], slots[1, 3], slots[0, 4]))
            {
                winnings += CalculateLineWinnings(slots[0, 0], symbolMultipliers, gambleAmount);
            }

            // Bottom-mid v line
            if (CheckLine(slots[4, 0], slots[3, 1], slots[2, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += CalculateLineWinnings(slots[4, 0], symbolMultipliers, gambleAmount);
            }

            // Bottom trapezoid line
            if (CheckLine(slots[4, 0], slots[3, 1], slots[3, 2], slots[3, 3], slots[4, 4]))
            {
                winnings += CalculateLineWinnings(slots[4, 0], symbolMultipliers, gambleAmount);
            }

            // Mid-top v line
            if (CheckLine(slots[2, 0], slots[1, 1], slots[0, 2], slots[1, 3], slots[2, 4]))
            {
                winnings += CalculateLineWinnings(slots[2, 0], symbolMultipliers, gambleAmount);
            }
            
            // Mid-bottom v line
            if (CheckLine(slots[2, 0], slots[3, 1], slots[4, 2], slots[3, 3], slots[2, 4]))
            {
                winnings += CalculateLineWinnings(slots[2, 0], symbolMultipliers, gambleAmount);
            }

            return winnings;
        }

        // Helper method to check if all elements in a line are the same
        private static bool CheckLine(params string[] symbols)
        {
            return symbols.All(symbol => symbol == symbols[0]);
        }

        private static int CalculateLineWinnings(string symbol, Dictionary<string, int> symbolMultipliers, int betAmount)
        {
            betAmount *= symbolMultipliers.TryGetValue(symbol, out int multiplier) ? multiplier : 1;

            return betAmount;
        }

        private static string updateScreen(string[,] slots, int betAmount)
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
                $"Spinning with {betAmount} credits... no luck!\n";
        }

            private static string emptyScreen(string[,] slots)
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
                    "Out of credits!\n";
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
        static bool homeModified = false;
        static bool leaving = false;

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
                else
                {
                    // Make custom error node here
                    __instance.terminalNodes.specialNodes[11] = savedNode;
                }
            }
        }

        [HarmonyPatch("PressESC")]
        [HarmonyPrefix]
        static void checkEsc(ref InputAction.CallbackContext context)
        {
            if(context.action.name == "OpenMenu")
            {
                leaving = true;
            }
        }

        [HarmonyPatch("BeginUsingTerminal")]
        [HarmonyPrefix]
        static void resetEsc()
        {
            leaving = false;
        }

        [HarmonyPatch("OnSubmit")]
        [HarmonyPrefix]
        static void resubmitBet(ref Terminal __instance)
        {
            if (__instance.textAdded == 0 && __instance.currentNode.terminalEvent == "spinGamba" && !leaving)
            {
                __instance.LoadNewNode(__instance.currentNode);
            }
        }

        [HarmonyPatch("LoadNewNode")]
        [HarmonyPrefix]
        static void spinGamba(ref TerminalNode node, ref Terminal __instance)
        {
            if (node.terminalEvent == "spinGamba")
            {
                __instance.terminalNodes.specialNodes[11] = savedNode;
                if (__instance.groupCredits - betAmount >= 0 && betAmount > 0)
                {
                    __instance.groupCredits -= betAmount;
                    node.displayText = updateScreen(SpinSlots(slots), betAmount);
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
                    __instance.SyncGroupCreditsServerRpc(__instance.groupCredits, __instance.numberOfItemsInDropship);
                } else
                {
                    node.displayText = emptyScreen(SpinSlots(slots));
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

            if(!homeModified)
            {
                string home = __instance.terminalNodes.specialNodes[1].displayText;
                __instance.terminalNodes.specialNodes[1].displayText = home.Substring(0, home.Length - 2) + "[Gamba Mod]\nType \"slots\" to begin your gambling career!\n";
                homeModified = true;
            }

            foreach (TerminalKeyword keyword in __instance.terminalNodes.allKeywords)
            {
                Console.WriteLine(keyword.specialKeywordResult.displayText);
                if (keyword.word.Equals("other"))
                {
                    int ind = keyword.specialKeywordResult.displayText.IndexOf("t.");
                    string word = keyword.specialKeywordResult.displayText;
                    keyword.specialKeywordResult.displayText = word.Substring(0, ind + 2) + "\n\n>SLOTS\nGamble company credits in the slot machine.\n";
                }
            }
        }
    }
}
