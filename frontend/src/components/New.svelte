<script>
    import { createHunt as send } from "../api";
    import { navigateTo } from "../naviagtion";
    let step = 0;
    let type;
    let game;
    let target = "";
    let created = false;

    function selectType(selected) {
        type = selected;
        step = 2;
    }

    function selectGame(selected) {
        game = selected;
        step = 1;
    }

    async function createHunt() {
        created = true;
        var response = await send(game, type, target);
        navigateTo("/hunt/" + response);
    }
</script>

<h1>Start new hunt</h1>

{#if game}<p>Game: {game}</p>{/if}
{#if type}<p>Type: {type}</p>{/if}

{#if step === 0}
    <p>Select game:</p>
    <button on:click={() => selectGame("Fire Red")}>Fire Red</button>
    <button on:click={() => selectGame("Heart Gold")}>Heart Gold</button>
{:else if step === 1}
    <p>Select type of hunt:</p>
    <button on:click={() => selectType("Soft reset")}>Soft reset</button>
    <button on:click={() => selectType("Random encounter")}
        >Random encounter</button
    >
{:else if step === 2}
    {#if type === "Soft reset"}
        <p>Select target to hunt:</p>
        <label for="target">
            Target: <input id="target" type="text" bind:value={target} />
        </label>
    {/if}

    {#if !created}
        <button on:click={createHunt}>Start!</button>
    {:else}
        <p>Creating...</p>
    {/if}
{/if}
