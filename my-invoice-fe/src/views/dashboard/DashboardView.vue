<template>
  <div class="workflow-wrapper">
    <div class="top-nav">
      <div class="tab-group">
        <button class="tab-btn">Tổng quan</button>
        <button class="tab-btn">Hướng dẫn sử dụng</button>
        <button class="tab-btn active">Quy trình nghiệp vụ</button>
      </div>
    </div>

    <div class="sub-nav">
      <div class="sub-item active">Hóa đơn điện tử</div>
      <div class="sub-item">Hóa đơn điện tử khởi tạo từ máy tính tiền</div>
    </div>

    <div class="canvas-container">
      <div class="workflow-canvas">
        <svg class="svg-layer" viewBox="0 0 1150 550">
          <defs>
            <marker
              id="arrow"
              viewBox="0 0 10 10"
              refX="8"
              refY="5"
              markerWidth="6"
              markerHeight="6"
              orient="auto-start-reverse"
            >
              <path d="M 0 0 L 10 5 L 0 10 z" fill="#cbd5e1" />
            </marker>
            <marker id="dot" viewBox="0 0 10 10" refX="5" refY="5" markerWidth="6" markerHeight="6">
              <circle cx="5" cy="5" r="3" fill="#cbd5e1" />
            </marker>
          </defs>
          <path
            d="M 110 120 C 160 120, 160 275, 200 275 L 320 275"
            class="line"
            marker-start="url(#dot)"
            marker-end="url(#arrow)"
          />
          <path d="M 110 440 C 160 440, 160 275, 200 275" class="line" marker-start="url(#dot)" />
          <path d="M 270 275 L 270 390" class="line" marker-end="url(#arrow)" />
          <path d="M 430 275 L 485 275" class="line" marker-end="url(#arrow)" />
          <path d="M 560 275 L 635 275" class="line" marker-end="url(#arrow)" />
          <path d="M 710 275 L 785 275" class="line" marker-end="url(#arrow)" />
          <path d="M 865 275 L 910 275" class="line" />
          <path d="M 920 275 L 930 265 L 940 275 L 930 285 Z" fill="#cbd5e1" />
          <path
            d="M 940 275 C 980 275, 980 120, 1025 120"
            class="line"
            marker-start="url(#dot)"
            marker-end="url(#arrow)"
          />
          <path
            d="M 940 275 L 1025 275"
            class="line"
            marker-start="url(#dot)"
            marker-end="url(#arrow)"
          />
          <path
            d="M 940 275 C 980 275, 980 440, 1025 440"
            class="line"
            marker-start="url(#dot)"
            marker-end="url(#arrow)"
          />
        </svg>

        <div
          v-for="node in nodes"
          :key="node.id"
          class="node"
          :style="{ left: node.left + 'px', top: node.top + 'px' }"
        >
          <div class="icon-box">
            <div v-if="node.type === 'doc'" class="custom-doc">
              <div class="doc-text">{{ node.docTitle }}</div>
              <div class="doc-line short"></div>
              <div class="doc-line long"></div>
              <div class="doc-line mid"></div>
            </div>

            <div v-if="node.type === 'monitor'" class="custom-monitor">
              <div class="screen">
                <div class="screen-inner">
                  <div class="screen-text">HÓA ĐƠN</div>
                  <div class="doc-line short"></div>
                  <div class="doc-line mid"></div>
                </div>
              </div>
              <div class="neck"></div>
              <div class="base"></div>
            </div>

            <div v-if="node.badge" class="badge" :style="{ backgroundColor: node.badge.color }">
              <component :is="node.badge.icon" size="14" stroke-width="3" />
            </div>
            <div v-if="node.badges" class="badge-list">
              <div
                v-for="(b, i) in node.badges"
                :key="i"
                class="badge"
                :style="{ backgroundColor: b.color }"
              >
                <component :is="b.icon" size="14" stroke-width="3" />
              </div>
            </div>
          </div>
          <div class="node-title">{{ node.title }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { Pencil, Plus, Send, Volume2, Check } from 'lucide-vue-next'

const nodes = [
  {
    id: 1,
    title: 'Đăng ký sử dụng\nHĐĐT',
    docTitle: 'ĐĂNG KÝ',
    type: 'doc',
    badge: { icon: Pencil, color: '#10b981' },
    top: 80,
    left: 20,
  },
  {
    id: 2,
    title: 'Khởi tạo mẫu',
    docTitle: 'MẪU HĐ',
    type: 'doc',
    badge: { icon: Plus, color: '#3b82f6' },
    top: 400,
    left: 20,
  },
  {
    id: 3,
    title: 'Thay đổi thông tin\nđăng ký sử dụng',
    docTitle: 'THAY ĐỔI',
    type: 'doc',
    badge: { icon: Pencil, color: '#10b981' },
    top: 400,
    left: 210,
  },
  {
    id: 4,
    title: 'Lập và phát hành\nHĐĐT',
    type: 'monitor',
    badge: { icon: Check, color: '#10b981' },
    top: 240,
    left: 340,
  },
  {
    id: 5,
    title: 'Gửi hóa đơn để\nCQT cấp mã',
    docTitle: 'GỬI HÓA ĐƠN',
    type: 'doc',
    badge: { icon: Send, color: '#ef4444' },
    top: 240,
    left: 495,
  },
  {
    id: 6,
    title: 'Gửi hóa đơn cho\nngười mua',
    docTitle: 'GỬI HÓA ĐƠN',
    type: 'doc',
    badge: { icon: Send, color: '#3b82f6' },
    top: 240,
    left: 645,
  },
  {
    id: 7,
    title: 'Lập văn bản thỏa thuận/thông báo\nvới người mua về HĐ sai sót',
    docTitle: 'VĂN BẢN/\nTHÔNG BÁO',
    type: 'doc',
    badges: [
      { icon: Plus, color: '#3b82f6' },
      { icon: Volume2, color: '#f59e0b' },
    ],
    top: 240,
    left: 795,
  },
  {
    id: 8,
    title: 'Thông báo HĐĐT\nlập sai với CQT',
    docTitle: 'THÔNG BÁO',
    type: 'doc',
    badge: { icon: Volume2, color: '#f59e0b' },
    top: 80,
    left: 1040,
  },
  {
    id: 9,
    title: 'Lập HĐ thay thế',
    docTitle: 'THAY THẾ',
    type: 'doc',
    badge: { icon: Plus, color: '#3b82f6' },
    top: 240,
    left: 1040,
  },
  {
    id: 10,
    title: 'Lập HĐ điều chỉnh',
    docTitle: 'ĐIỀU CHỈNH',
    type: 'doc',
    badge: { icon: Pencil, color: '#10b981' },
    top: 400,
    left: 1040,
  },
]
</script>

<style scoped>
/* Reset & Base */
.workflow-wrapper {
  min-height: 100vh;
  font-family: sans-serif;
  color: #334155;
}

/* Nav Styles */
.top-nav {
  display: flex;
  justify-content: center;
  padding-bottom: 16px;
  background-color: #eef1f5;
}
.tab-group {
  display: flex;
  background-color: #e2e6eb;
  padding: 4px;
  border-radius: 999px;
}
.tab-btn {
  padding: 8px 24px;
  border-radius: 999px;
  border: none;
  font-size: 14px;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  background: transparent;
}
.tab-btn.active {
  background: white;
  color: #0f172a;
  font-weight: bold;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.sub-nav {
  background: white;
  padding: 16px 32px 0;
  border-top-left-radius: 12px;
  border-top-right-radius: 12px;
  border-bottom: 1px solid #e2e8f0;
  display: flex;
  gap: 32px;
}
.sub-item {
  padding-bottom: 12px;
  font-size: 14px;
  color: #64748b;
  cursor: pointer;
  border-bottom: 3px solid transparent;
}
.sub-item.active {
  color: #007aff;
  font-weight: 600;
  border-bottom-color: #007aff;
}

/* Canvas Area */
.canvas-container {
  display: flex;
  justify-content: center;
  width: 100%;
}
.workflow-canvas {
  position: relative;
  width: 100%;
  height: 550px;
  background: white;
  border-bottom-left-radius: 12px;
  border-bottom-right-radius: 12px;
  border: 1px solid #f1f5f9;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

/* SVG Lines */
.svg-layer {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 0;
}
.line {
  stroke: #cbd5e1;
  stroke-width: 2.5;
  fill: none;
}

/* Node Styling */
.node {
  position: absolute;
  width: 128px;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  z-index: 10;
  cursor: pointer;
}
.icon-box {
  position: relative;
  margin-bottom: 12px;
}

/* Custom CSS Icons */
.custom-doc {
  width: 60px;
  height: 76px;
  background: white;
  border: 2px solid #cbd5e1;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 8px;
  transition: border-color 0.2s;
}
.node:hover .custom-doc,
.node:hover .screen {
  border-color: #007aff;
}
.doc-text {
  font-size: 8px;
  font-weight: bold;
  color: #94a3b8;
  margin-bottom: 6px;
  line-height: 1;
}
.doc-line {
  height: 2px;
  background: #e2e8f0;
  border-radius: 4px;
  margin-bottom: 4px;
}
.doc-line.short {
  width: 65%;
}
.doc-line.long {
  width: 85%;
}
.doc-line.mid {
  width: 50%;
}

.custom-monitor {
  display: flex;
  flex-direction: column;
  align-items: center;
}
.screen {
  width: 76px;
  height: 54px;
  background: #f8fafc;
  border: 2.5px solid #cbd5e1;
  border-radius: 8px;
  display: flex;
  justify-content: center;
  align-items: center;
}
.screen-inner {
  width: 56px;
  height: 36px;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 2px;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 6px;
}
.screen-text {
  font-size: 7px;
  font-weight: bold;
  color: #94a3b8;
  margin-bottom: 4px;
}
.neck {
  width: 18px;
  height: 10px;
  background: #e2e8f0;
  border-left: 2.5px solid #cbd5e1;
  border-right: 2.5px solid #cbd5e1;
}
.base {
  width: 44px;
  height: 4px;
  background: #cbd5e1;
  border-radius: 999px;
}

/* Badges */
.badge {
  position: absolute;
  bottom: -8px;
  right: -12px;
  width: 26px;
  height: 26px;
  border-radius: 50%;
  border: 2px solid white;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}
.badge-list {
  position: absolute;
  bottom: -8px;
  right: -24px;
  display: flex;
  gap: 2px;
}

.node-title {
  font-size: 13px;
  font-weight: 500;
  color: #475569;
  line-height: 1.4;
  white-space: pre-line;
}
.node:hover .node-title {
  color: #007aff;
}
</style>
