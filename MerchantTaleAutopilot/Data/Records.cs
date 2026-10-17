using System;
using System.Linq;

namespace MerchantTaleAutopilot.Data;

public enum StepKind
{
    Pick,
    Do,
    Fight,
    Avoid,
    Talk,
    Finish,
}

public readonly record struct Step(string Text, StepKind Kind);

public sealed class RecordData
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public string Brief { get; init; } = string.Empty;
    public Step[] Steps { get; init; } = Array.Empty<Step>();
}

public static class RouteData
{
    public static readonly (string Name, int[] Ids)[] Groups =
    {
        ("Residential - Left", new[] { 1, 2, 3, 4 }),
        ("Market - Right", new[] { 5, 6, 7, 8 }),
        ("Secret", new[] { 13 }),
        ("Gate - Middle", new[] { 9, 10, 11, 12 }),
    };

    public static readonly RecordData[] Records =
    {
        new()
        {
            Id = 1,
            Title = "A Land of Abundance",
            Route = "RESIDENTIAL",
            Brief = "Left route, either apple choice, break the fence, then ATTACK Willful Wind to stop it fleeing.",
            Steps = new[]
            {
                new Step("Pick the LEFT path at the Font of Paths (Residential).", StepKind.Pick),
                new Step("Apple fork: the apple path or the other one, either works. It does not change which record you get.", StepKind.Pick),
                new Step("After the first boss, interact with the Broken Fence next to your personal loot chest to open the side path.", StepKind.Do),
                new Step("Follow the path to the nest. When Willful Wind appears, attack it immediately to interrupt Flee (this saves the chicks).", StepKind.Fight),
                new Step("Finish the rest of the dungeon and Pari of Plenty as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 2,
            Title = "A Carpet Soars",
            Route = "RESIDENTIAL",
            Brief = "Left route, either apple choice, break the fence, then DO NOT attack Willful Wind - let it steal the apple and fly off.",
            Steps = new[]
            {
                new Step("Pick the LEFT path at the Font of Paths (Residential).", StepKind.Pick),
                new Step("Apple fork: either option.", StepKind.Pick),
                new Step("Interact with the Broken Fence and head through.", StepKind.Do),
                new Step("This time do NOT attack Willful Wind. Let it steal the apple and fly away.", StepKind.Avoid),
                new Step("Finish the rest of the dungeon and Pari of Plenty as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 3,
            Title = "A Foe of Flame",
            Route = "RESIDENTIAL",
            Brief = "Left route, either apple choice, IGNORE the fence, then stand by the railing so Ancient Aero destroys it.",
            Steps = new[]
            {
                new Step("Pick the LEFT path at the Font of Paths (Residential).", StepKind.Pick),
                new Step("Apple fork: either option.", StepKind.Pick),
                new Step("Ignore the Broken Fence completely and keep going along the main path through the gate.", StepKind.Avoid),
                new Step("Near the palace doors, find the two Airy Wisps. Do not kill them yet - stand next to the crumbling stone railing on the LEFT side of the door.", StepKind.Do),
                new Step("Let the Wisps cast Ancient Aero on you so the AoE destroys the railing.", StepKind.Fight),
                new Step("Take the newly opened path and finish the dungeon.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 4,
            Title = "Fragrant Fruits",
            Route = "RESIDENTIAL",
            Brief = "Left route, either apple choice, ignore the fence, then kill the Wisps normally and leave the railing intact.",
            Steps = new[]
            {
                new Step("Pick the LEFT path at the Font of Paths (Residential).", StepKind.Pick),
                new Step("Apple fork: either option.", StepKind.Pick),
                new Step("Ignore the Broken Fence and stay on the main path.", StepKind.Do),
                new Step("Kill the Airy Wisps normally. Do not let any AoE hit the stone railing.", StepKind.Avoid),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 5,
            Title = "The Sunlit Expanse",
            Route = "MARKET",
            Brief = "Right route, take the seaweed, let Yawning Maw's Inhale absorb all four coloured rock piles, then kill it.",
            Steps = new[]
            {
                new Step("Pick the RIGHT path at the Font of Paths (Market).", StepKind.Pick),
                new Step("When Y'nazqha asks, choose: \"The seaweed won't lead us astray.\"", StepKind.Talk),
                new Step("Find Yawning Maw (the purple goobbue) and position so its Inhale cone absorbs ALL FOUR coloured rock piles on the ground.", StepKind.Do),
                new Step("Once all four are absorbed, kill it.", StepKind.Fight),
                new Step("Finish the rest of the dungeon and Darya the Sea-maid as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 6,
            Title = "Treasure at the Ocean's Floor",
            Route = "MARKET",
            Brief = "Right route, take the seaweed, then kill Yawning Maw early - before it finishes inhaling.",
            Steps = new[]
            {
                new Step("Pick the RIGHT path at the Font of Paths (Market).", StepKind.Pick),
                new Step("Choose: \"The seaweed won't lead us astray.\"", StepKind.Talk),
                new Step("Kill Yawning Maw BEFORE it finishes inhaling the rocks. Even one pile left over counts as a success.", StepKind.Avoid),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 7,
            Title = "Rule of the Radiant Sea",
            Route = "MARKET",
            Brief = "Right route, walk the beach, then interact with the Giant Conch Shell.",
            Steps = new[]
            {
                new Step("Pick the RIGHT path at the Font of Paths (Market).", StepKind.Pick),
                new Step("Choose: \"I'd like to take a walk along the beach.\"", StepKind.Talk),
                new Step("In Seven-shade Sands, find the Giant Conch Shell on the north side near the two Shell Dwellers and interact with it.", StepKind.Do),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 8,
            Title = "The Ocean's Bounty",
            Route = "MARKET",
            Brief = "Right route, walk the beach, then leave the Giant Conch Shell alone and go straight to the boss.",
            Steps = new[]
            {
                new Step("Pick the RIGHT path at the Font of Paths (Market).", StepKind.Pick),
                new Step("Choose: \"I'd like to take a walk along the beach.\"", StepKind.Talk),
                new Step("Do not interact with the Giant Conch Shell. Walk past it straight to the boss.", StepKind.Avoid),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 9,
            Title = "Valley of Extremes",
            Route = "GATE",
            Brief = "Middle route, follow the mushrooms, use Magnetic Rock to light all three spiked rock clusters.",
            Steps = new[]
            {
                new Step("Pick the MIDDLE path at the Font of Paths (Gate).", StepKind.Pick),
                new Step("Choose: \"Let the mushrooms guide us!\"", StepKind.Talk),
                new Step("Inside the Passage of Earth cave, Stone Puppet marks the tank with a purple AoE called Magnetic Rock.", StepKind.Do),
                new Step("Lead that AoE next to the three spiked rock clusters so the explosion hits them. Each one glows purple when charged.", StepKind.Fight),
                new Step("Once all three are lit you will see \"The air's magnetism has shifted...\" - continue to the boss.", StepKind.Do),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 10,
            Title = "A Blade Engraven",
            Route = "GATE",
            Brief = "Middle route, follow the mushrooms, kill Stone Puppet normally and keep Magnetic Rock off every cluster.",
            Steps = new[]
            {
                new Step("Pick the MIDDLE path at the Font of Paths (Gate).", StepKind.Pick),
                new Step("Choose: \"Let the mushrooms guide us!\"", StepKind.Talk),
                new Step("Kill Stone Puppet normally. Do not let Magnetic Rock's AoE touch any of the rock clusters.", StepKind.Avoid),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 11,
            Title = "Master of the Unyielding Blade",
            Route = "GATE",
            Brief = "Middle route, examine the floating rocks, charge all three glowing rocks - the upper one via the west path.",
            Steps = new[]
            {
                new Step("Pick the MIDDLE path at the Font of Paths (Gate).", StepKind.Pick),
                new Step("Choose: \"Those floating rocks are worth a closer look.\"", StepKind.Talk),
                new Step("In the Passage of Sky, find the three glowing rocks jutting from the platforms and have the party stand next to each until it is fully charged.", StepKind.Do),
                new Step("The upper rock requires taking the path around the WEST side of the room.", StepKind.Do),
                new Step("Once all three glow and you see \"The air's magnetism has shifted...\", continue to the boss.", StepKind.Do),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 12,
            Title = "In a Harsh Nature",
            Route = "GATE",
            Brief = "Middle route, examine the floating rocks, but charge none of them - clear enemies away from the rocks.",
            Steps = new[]
            {
                new Step("Pick the MIDDLE path at the Font of Paths (Gate).", StepKind.Pick),
                new Step("Choose: \"Those floating rocks are worth a closer look.\"", StepKind.Talk),
                new Step("Do not charge any of the large rocks in the Passage of Sky. Kill enemies well away from them.", StepKind.Avoid),
                new Step("Finish the dungeon as normal.", StepKind.Finish),
            },
        },
        new()
        {
            Id = 13,
            Title = "The Eye of the Beholder",
            Route = "SECRET",
            Brief = "Right route, seaweed, turn right into the Shimmering Stage, /bow, say \"I am returned!\", then step the panels in order.",
            Steps = new[]
            {
                new Step("Pick the RIGHT path at the Font of Paths (Market).", StepKind.Pick),
                new Step("Choose: \"The seaweed won't lead us astray.\"", StepKind.Talk),
                new Step("Before the usual boss arena, turn RIGHT into the Shimmering Stage - the round platform.", StepKind.Do),
                new Step("Use /bow on the platform. Five coloured panels appear.", StepKind.Talk),
                new Step("In Say chat, type exactly: I am returned!   (with the exclamation mark). The panels light up with shapes.", StepKind.Talk),
                new Step("Step on the panels in this order: Coral (green) -> Conch (purple) -> Starfish (pink) -> Pearl (blue).", StepKind.Do),
                new Step("A secret bridge opens. Cross it and defeat Deadly Dandan.", StepKind.Fight),
                new Step("Warning: the whole ritual must be finished BEFORE crossing the Tangled Seaweed Barrier. Miss one step and the secret route is gone for that run.", StepKind.Avoid),
            },
        },
    };

    public static RecordData Get(int id)
    {
        return Records.FirstOrDefault(r => r.Id == id) ?? Records[0];
    }
}
