<script setup>
const model = defineModel()
//#region Props
defineProps({
  value: {
    type: [String, Number, Boolean, Object],
    default: undefined,
  },
})
//#end region
</script>

<template>
  <label class="ms-checkbox-wrapper" @click.stop>
    <input type="checkbox" v-model="model" :value="value" class="ms-native" />
    <span class="ms-checkbox" aria-hidden="true"></span>
  </label>
</template>

<style scoped>
.ms-checkbox-wrapper {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.ms-native {
  position: absolute;
  opacity: 0;
  width: 0;
  height: 0;
  margin: 0;
  padding: 0;
  pointer-events: none;
}

.ms-checkbox {
  width: 16px;
  height: 16px;
  box-sizing: border-box;
  border: 1.5px solid #d1d5db;
  border-radius: 2px;
  background: white;
  position: relative;
  display: inline-block;
}

.ms-checkbox-wrapper:hover .ms-checkbox {
  border-color: var(--color-primary);
}

.ms-native:checked + .ms-checkbox {
  background-color: var(--color-primary);
  border-color: var(--color-primary);
}

.ms-checkbox::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 4px;
  height: 8px;
  border-style: solid;
  border-width: 0 1.5px 1.5px 0;
  border-color: white;
  transform: translate(-50%, -60%) rotate(45deg) scale(0);
  transform-origin: center;
}

.ms-native:checked + .ms-checkbox::before {
  transform: translate(-50%, -60%) rotate(45deg) scale(1);
}

.ms-native:focus-visible + .ms-checkbox {
  outline: 2px solid var(--color-primary);
  outline-offset: 2px;
}
</style>
