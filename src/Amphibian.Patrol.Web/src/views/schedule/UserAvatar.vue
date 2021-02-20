<template>
    <div class="c-avatar">
        <template v-if="user && user.profileImageUrl">
          <img
            :src="user.profileImageUrl"
            class="c-avatar-img "
            />
          </template>
          <template v-if="!user || !user.profileImageUrl">
            <img
            src="img/avatars/patrol-50.jpg"
            class="c-avatar-img "
            />
        </template>
    </div>
</template>

<script>

export default {
  name:"UserAvatar",
  props: ['user'],
  components: {},
  data(){
    return {
    }
  },
}
</script>