<script>
    import { pathStore, navigateTo, createRelativePath } from "../naviagtion";

    export let path;
    export let relative = false;
    $: actualPath = relative ? createRelativePath($pathStore, path) : path;
</script>

<div on:click={() => navigateTo(actualPath)}>
    <slot>{path}</slot>
</div>

<style>
    div {
        cursor: pointer;
    }
</style>
