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
  arrow: {
    type: Boolean,
    default: true,
  },
})
//#end region
</script>

<template>
  <div class="tooltip-container">
    <slot></slot>

    <div v-if="content" class="tooltip-content" :class="position">
      {{ content }}
      <div v-if="arrow" class="tooltip-arrow"></div>
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
  z-index: 9999;
  visibility: hidden;
  opacity: 0;
  position: absolute;
  background-color: #111827;
  color: #fff;
  text-align: center;
  padding: 6px 10px;
  border-radius: 2px;
  white-space: nowrap;
  font-size: inherit;
  font-weight: 400;
  pointer-events: none;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  transition:
    opacity 0.2s,
    visibility 0.2s,
    transform 0.2s;
  font-size: 14px;
}

.tooltip-content.top {
  bottom: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-bottom: 10px;
}

.tooltip-content.bottom {
  top: 100%;
  left: 50%;
  transform: translateX(-50%);
  margin-top: 10px;
}

.tooltip-arrow {
  position: absolute;
  left: 50%;
  margin-left: -5px;
  border-width: 5px;
  border-style: solid;
}

.tooltip-content.top .tooltip-arrow {
  top: 100%;
  border-color: #111827 transparent transparent transparent;
}

.tooltip-content.bottom .tooltip-arrow {
  bottom: 100%;
  border-color: transparent transparent #111827 transparent;
}

.tooltip-container:hover .tooltip-content {
  visibility: visible;
  opacity: 1;
}
</style>
