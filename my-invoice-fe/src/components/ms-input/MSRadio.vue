<template>
  <input type="radio" :name="name" class="ms-radio" :value="value" v-model="model" />
</template>
<script setup>
//#region Props & Model
const props = defineProps(['name', 'value'])
const model = defineModel()
//#end region
</script>

<style scoped>
.ms-radio {
  padding: 0 !important;
  appearance: none;
  -webkit-appearance: none;

  width: 16px;
  height: 16px;

  border: 2px solid #7b7b7b;
  border-radius: 50% !important;
  background-color: white;
  cursor: pointer;
  margin: 0;
  display: grid;
  place-content: center;
}
.ms-radio:hover {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px var(--color-primary);
}

.ms-radio:checked {
  border-color: var(--color-primary);
  background-color: white;
}

.ms-radio::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
  transform: scale(0);

  background-color: var(--color-primary);
}

.ms-radio:checked::before {
  transform: scale(1);
}
</style>
