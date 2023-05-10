<script>
    import {
        pathStore,
        matches,
        getWildCardValues,
        hasWildCard,
    } from "../naviagtion";

    export let path = "/";

    const hasWildCards = hasWildCard(path);
    let wildCards;
    $: match = matches($pathStore, path);
    $: {
        if (hasWildCards) {
            wildCards = getWildCardValues(path, $pathStore);
        }
    }
</script>

{#if match}
    <slot {wildCards} />
{/if}
