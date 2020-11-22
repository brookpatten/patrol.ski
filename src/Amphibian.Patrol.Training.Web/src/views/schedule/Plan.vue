<template>
    <div>
        <CCard>
            <GoogleLogin :params="googleParams" :onSuccess="onSuccess" :onFailure="onFailure">Login</GoogleLogin>
        </CCard>
        <CCard>
        </CCard>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> {{caption}}
            </slot>
            </CCardHeader>
            <CCardBody>
            <CDataTable
                :hover="hover"
                :striped="true"
                :bordered="true"
                :small="small"
                :fixed="fixed"
                :items="demo"
                :fields="demoFields"
                :items-per-page="small ? 10 : 5"
                :dark="dark"
                pagination
            >
                <template #status="{item}">
                <td>
                    <CBadge :color="getBadge(item.status)">{{item.status}}</CBadge>
                </td>
                </template>
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>
import GoogleLogin from 'vue-google-login';
export default {
  name: 'Plan',
  components: {
      GoogleLogin
  },
  data () {
    return {
        googleParams:{
            client_id: ''
        },
        plan: {
          skills: [
              {id:1, name: 'Gliding Wedge'},
              {id:2, name: 'Hockey Stop'},
          ],
          levels: [
              {id:1, name:'1'},
              {id:2, name:'2'}
          ],
          signatures: [],
      },
      demo: [
          {name:'Hockey Stop',lvl1:'',lvl2a:'',lvl2b:'',final:''},
          {name:'Gliding Wedge',lvl1:'',lvl2a:'',lvl2b:'',final:''}
      ],
      demoFields:[
          {key:'name', label:'Skill', stickyColumn:true},
          {key:'lvl1', label:'Level 1'},
          {key:'lvl2a', label:'Level 2+'},
          {key:'lvl2b', label:'Level 2+'},
          {key:'final', label:'Final'}
      ]
    }
  },
  methods: {
        onSuccess(googleUser) {
            console.log(googleUser);
 
            // This only gets the user information: id, name, imageUrl and email
            console.log(googleUser.getBasicProfile());

            this.$http.get('authentication/google/test')
                .then(response => {
                    console.log(response);
                }).catch(response => {
                    console.log(response);
                })
        },
        onFailure(err) {
            console.log(err);
        }
  }
}
</script>
