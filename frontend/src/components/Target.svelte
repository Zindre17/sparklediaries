<script>
    import { fetchHunt, completeHunt, addEncounters } from "../api";
    export let id;
    let override = false;

    function increment() {
        addEncounters(id, 1);
        hunt.encounters++;
    }
    function complete() {
        completeHunt(id);
    }
    let hunt;
    function receivedHunt(_, receivedHunt) {
        hunt = receivedHunt;
    }
</script>

{#await fetchHunt(id) then result}
    <div use:receivedHunt={result}>
        {#if hunt != null && hunt != undefined}
            <h1>{hunt.game}</h1>
            <h2>{hunt.type}</h2>
            {#if hunt.target} <h3>{hunt.target}</h3> {/if}
            <div>
                <span>Encounters: </span>
                {#if override}
                    <label>
                        <input type="number" bind:value={hunt.encounters} />
                        <button on:click={() => (override = false)}
                            >Update</button
                        >
                    </label>
                {:else}
                    <div class="row">
                        <p>{hunt.encounters}</p>
                        <button on:click={() => (override = true)}>edit</button>
                    </div>
                {/if}
            </div>
            {#if !hunt.completed}
                <div class="actions">
                    <button class="increment" on:click={increment}>+</button>
                    <button on:click={complete}>Found!</button>
                </div>
            {/if}
        {/if}
    </div>
{/await}

<style>
    .actions {
        display: grid;
        justify-content: center;
    }
    .row {
        display: grid;
        grid-template-columns: 1fr 50px;
    }

    .increment {
        width: 200px;
        font-size: 4rem;
        color: white;
        font-weight: bold;
        text-shadow: 3px 3px 0 #333;
    }

    .actions > button {
        background-image: linear-gradient(
            rgb(67, 205, 230),
            0%,
            rgb(67, 205, 230),
            60%,
            rgb(127, 255, 255),
            60%,
            rgb(127, 255, 255),
            65%,
            rgb(67, 205, 230),
            65%,
            rgb(67, 205, 230),
            70%,
            rgb(127, 255, 255),
            70%,
            rgb(127, 255, 255)
        );
        border-style: none;
        margin: 0.5rem auto;
        padding: 1rem 2rem;
        /* border: 0.2rem solid transparent; */
    }

    button:hover,
    button:focus {
        border: 2px solid orange;
    }
</style>
