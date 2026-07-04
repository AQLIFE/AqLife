<script lang="ts" setup>
import WishCard from '@/components/WishCard.vue'
import { ElCol } from 'element-plus'

const wishs = [
  {
    imageLink:
      'https://openfile.meizu.com/group1/M00/0F/D0/Cgbj0WgivdmALYksAAggNl4t5Vs081.png@480x480.jpg',
    wishName: 'Meizu 21Pro',
  },
  {
    imageLink: 'https://www.colorful.cn/uploads/AttachFile/201907121402254150.JPG',
    wishName: 'Colorful iGame GeForce RTX 2060 SUPER Ultra',
  },
  {
    imageLink:
      'https://shared.cdn.queniuqe.com/store_item_assets/steam/apps/275850/9ecc87d1062c690c96adeebd33ed761c1bda842f/header_alt_assets_25.jpg?t=1761138171',
    wishName: "No Man's Sky",
  },
  // { imageLink: 'https://shared.cdn.queniuqe.com/store_item_assets/steam/apps/457140/header_schinese.jpg?t=1750118450', wishName: '缺氧' },
  {
    imageLink:
      'https://shared.cdn.queniuqe.com/store_item_assets/steam/apps/255710/header_schinese.jpg?t=1758712642',
    wishName: 'City:SkyLines',
  },
]
</script>

<template>
  <ElRow>
    <ElCol :span="8" v-for="(item, index) in wishs" :key="index">
      <WishCard :wish-image="item.imageLink" :wish-name="item.wishName" />
    </ElCol>
  </ElRow>
</template>

<style scoped>
.el-row {
  /* background-color: var(--back_color_lv1); */
}
</style>
