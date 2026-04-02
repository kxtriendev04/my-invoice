<script setup>
//#region Props
defineProps({
  content: {
    type: [String, Number],
    default: '',
  },
  position: {
    type: String,
    default: 'top',
  },
})
//#end region
</script>

<template>
  <div class="tooltip-container">
    <slot></slot>

    <div class="tooltip-content" :class="position">
      {{ content }}
    </div>
  </div>
</template>

<style scoped>
.tooltip-container {
  position: relative;
  cursor: pointer;
  display: flex;
  width: 100%;
  height: 100%;
  align-items: center;
}

.tooltip-content {
  z-index: 100;
  visibility: hidden;
  opacity: 0;
  position: absolute;
  background-color: #374151;
  color: #fff;
  text-align: center;
  padding: 3px 8px;
  border-radius: 4px;
  white-space: nowrap;
  font-size: 12px;
  font-weight: 500;
  transition:
    opacity 0.2s,
    visibility 0.2s;
  pointer-events: none;
}

.tooltip-content.top {
  bottom: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-bottom: 8px;
}

.tooltip-content.bottom {
  top: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-top: 8px;
}

.tooltip-container:hover .tooltip-content {
  visibility: visible;
  opacity: 1;
}
</style>
