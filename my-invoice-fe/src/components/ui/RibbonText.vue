<template>
  <div class="ribbon-banner" :class="`ribbon-${color}`">
    <span class="bulb-icon">
      <slot name="icon">{{ icon }}</slot>
    </span>

    <span class="ribbon-text">
      <slot>
        <i>
          {{ text }}
          <a v-if="url" :href="url" class="action-link" target="_blank">{{ actionText }}</a>
          <u v-else @click="$emit('action-click')" class="action-link">{{ actionText }}</u>
        </i>
      </slot>
    </span>
  </div>
</template>

<script setup>
defineProps({
  icon: {
    type: String,
    default: '💡',
  },
  text: {
    type: String,
    default: 'Phim hướng dẫn.',
  },
  actionText: {
    type: String,
    default: 'Xem ngay!',
  },
  url: {
    type: String,
    default: '',
  },
  color: {
    type: String,
    default: 'green', // 'green' | 'orange'
    validator: (value) => ['green', 'orange'].includes(value),
  },
})

defineEmits(['action-click'])
</script>

<style scoped>
/* ==========================================
   COMPONENT: RIBBON BANNER (KÍCH THƯỚC NHỎ GỌN)
   ========================================== */
.ribbon-banner {
  position: relative;
  /* Kéo dải băng ra ngoài mép giấy. 
     Padding cha 32px + nếp gấp 10px = 42px */
  margin-left: -42px;
  margin-bottom: 20px;

  display: inline-flex;
  align-items: center;

  /* Mặc định màu xanh lá */
  background: linear-gradient(90deg, #00c16e, #00d378);
  color: #fff;
  /* Giảm padding để dải băng thanh mảnh hơn */
  padding: 6px 16px 6px 12px;

  /* Bo tròn hình viên thuốc nhỏ lại */
  border-radius: 0 16px 16px 0;
  box-shadow: 2px 2px 5px rgba(0, 0, 0, 0.1);
  z-index: 2;
}

/* Màu cam cho InvoiceCreation */
.ribbon-orange {
  background: linear-gradient(90deg, #ff7a45, #ff9c6e);
}
.ribbon-orange::before {
  border-top-color: #e07835 !important; /* Màu đậm hơn cho fold effect */
}

/* TẠO NẾP GẤP (FOLD EFFECT) */
.ribbon-banner::before {
  content: '';
  position: absolute;
  left: 0;
  bottom: -10px; /* Độ dời xuống giảm còn 10px */

  /* Vẽ tam giác nhỏ hơn */
  border-top: 10px solid #008744;
  border-left: 10px solid transparent;
}

/* Tuỳ chỉnh Text và Icon */
.ribbon-banner .bulb-icon {
  margin-right: 6px; /* Icon gần chữ hơn */
  font-size: 14px; /* Icon bé lại */
}

.ribbon-banner .ribbon-text {
  font-size: 13px; /* Chữ bé lại, khớp với font chuẩn */
}

.action-link {
  color: #fff;
  text-underline-offset: 2px; /* Đường gạch chân sát chữ hơn */
  cursor: pointer;
  text-decoration: underline;
}

.action-link:hover {
  opacity: 0.8;
}
</style>
