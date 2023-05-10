<script>
    import Navigate from "./Navigate.svelte";

    export let hunt;
</script>

<Navigate path="hunt/{hunt.id}">
    <div class="container">
        <p>{hunt.type}</p>
        <p>{hunt.target ? hunt.target : "No target"}</p>
        <p>Encounters: {hunt.encounters}</p>
    </div>
</Navigate>

<style>
    .container {
        border: 1px solid black;
        border-radius: 5px;
    }
    .container:hover {
        border-color: orangered;
    }
</style>
