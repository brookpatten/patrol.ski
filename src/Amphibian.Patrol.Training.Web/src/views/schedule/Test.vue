<template>
    <div>
        <!--<CCard>
            <GoogleLogin :params="googleParams" :onSuccess="onSuccess" :onFailure="onFailure">Login</GoogleLogin>
        </CCard>
        <CCard>
            <CButton v-on:click="test()">Test</CButton>
        </CCard>-->
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> {{caption}}
            </slot>
            </CCardHeader>
            <CCardBody>
            
            <table class="table table-striped">
                <thead class="thead-dark">
                <draggable v-model="headers" tag="tr">
                    <th v-for="header in headers" :key="header" scope="col">
                    {{ header }}
                    </th>
                </draggable>
                </thead>
                <draggable v-model="list" tag="tbody">
                <tr v-for="item in list" :key="item.name">
                    <td scope="row">{{ item.id }}</td>
                    <td>{{ item.name }}</td>
                    <td>{{ item.sport }}</td>
                </tr>
                </draggable>
            </table>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>
import GoogleLogin from 'vue-google-login';
import draggable from 'vuedraggable';

export default {
  name: 'Test',
  components: {
      GoogleLogin,
      draggable
  },
  data () {
    return {
        googleParams:{
            client_id: ''
        },
        headers: ["id", "name", "sport"],
        list: [
            { id: 1, name: "Abby", sport: "basket" },
            { id: 2, name: "Brooke", sport: "foot" },
            { id: 3, name: "Courtenay", sport: "volley" },
            { id: 4, name: "David", sport: "rugby" }
        ],
        dragging: false
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
