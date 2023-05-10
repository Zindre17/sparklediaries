<script>
    import Navigate from "./components/Navigate.svelte";
    import New from "./components/New.svelte";
    import Route from "./components/Route.svelte";
    import { fetchActiveHunts } from "./api";
    import Target from "./components/Target.svelte";
    import Summary from "./components/Summary.svelte";

    let groupedHunts = {};
    async function getActiveHunts() {
        var hunts = await fetchActiveHunts();

        groupedHunts = {};

        for (let hunt of hunts) {
            let current = groupedHunts[hunt.game];
            if (current === undefined) {
                groupedHunts[hunt.game] = [hunt];
            } else {
                current.push(hunt);
            }
        }
    }
</script>

<main>
    <Route path="hunt/new">
        <New />
    </Route>
    <Route path="hunt/$id" let:wildCards>
        <Target id={wildCards.id} />
    </Route>
    <Route>
        <h1>Sparklediaries</h1>
        {#await getActiveHunts() then}
            <h2>Active hunts</h2>
            {#each Object.keys(groupedHunts) as game}
                <div class="game-group">
                    <h3>{game}</h3>
                    <div class="game-hunt-container">
                        {#each groupedHunts[game] as hunt}
                            <Summary {hunt} />
                        {/each}
                    </div>
                </div>
            {/each}
        {/await}
        <Navigate path="/hunt/new/">
            <button>Start new hunt</button>
        </Navigate>
    </Route>
</main>

<style>
    main {
        text-align: center;
        padding: 1rem;
        max-width: 240px;
        margin: 0 auto;
    }

    .game-group {
        display: flex column;
        margin: 2rem;
    }

    .game-hunt-container {
        display: grid;
        justify-content: center;
        gap: 2rem;
        grid-template-columns: repeat(auto-fit, 20rem);
    }
    h1 {
        color: #ff3e00;
        text-transform: uppercase;
        font-size: 2rem;
        font-weight: 100;
    }

    @media (min-width: 640px) {
        main {
            max-width: 800px;
        }
        h1 {
            font-size: 4rem;
        }
    }
</style>
