<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list-rich"/> Search
            </slot>
            </CCardHeader>
            <CCardBody>
                <CRow>
                  <CCol>
                    <label for="from">From Date</label>
                    <datepicker v-model="from" input-class="form-control" calendar-class="card"></datepicker>
                  </CCol>
                  <CCol>
                    <label for="to">To Date</label>
                    <datepicker v-model="to" input-class="form-control" calendar-class="card"></datepicker>
                  </CCol>
                  <CCol>
                      <label for="to">Route</label>
                      <CInput v-model="route" input-class="form-control"/>
                  </CCol>
                </CRow>
            </CCardBody>
        </CCard>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list-rich"/> Api/Day
            </slot>
            </CCardHeader>
            <CCardBody>
                <CChartLine
                    :datasets="daysDataset"
                    :labels="daysLabels"
                    :options="daysOptions"
                />
            </CCardBody>
        </CCard>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list-rich"/> Api/User
            </slot>
            </CCardHeader>
            <CCardBody>
                <CChartPie
                    :datasets="usersDataset"
                    :labels="usersLabels"
                />
            </CCardBody>
        </CCard>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list-rich"/> Api/Route
            </slot>
            </CCardHeader>
            <CCardBody>
                <CChartBar
                    :datasets="routesDataset"
                    :labels="routesLabels"
                />
            </CCardBody>
        </CCard>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list-rich"/> Route Performance
            </slot>
            </CCardHeader>
            <CCardBody>
                <CChartBar
                    :datasets="routePerformanceDataset"
                    :labels="routePerformanceLabels"
                    :options="routePerformanceOptions"
                />
            </CCardBody>
        </CCard>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-list-rich"/> Time/Route (Cumulative)
            </slot>
            </CCardHeader>
            <CCardBody>
                <CChartPie
                    :datasets="routeTimeCumulativeDataset"
                    :labels="routePerformanceLabels"
                />
            </CCardBody>
        </CCard>
    </div>
</template>

<script>
import { CChartLine } from '@coreui/vue-chartjs'
import { CChartPie } from '@coreui/vue-chartjs'
import { CChartBar } from '@coreui/vue-chartjs'
import Datepicker from 'vuejs-datepicker';

export default {
  name: 'SysAdminDashboard',
  components: {
      CChartLine,CChartPie,CChartBar,Datepicker
  },
  data () {
    return {
        from: new Date(new Date().getTime() - (86400000 * 30)),
        to: new Date(),
        route: '',
        routes: [],
        days: [],
        users: [],
        routePeformance: [],
        routePerformanceOptions:{
            scales: {
                xAxes:[ {
                    stacked: true
                }],
                yAxes: [{
                        stacked: true
                    }
                ]
            }
        }
    }
  },
  mounted: function(){
    this.load();
  },
  computed: {
      daysDataset: function(){
        return [
            {
                label: 'Api/Day',
                borderColor: 'rgb(0,255,0)',
                data: _.map(this.days,'count'),
                fill:false,
                lineTension: 0
            }
        ];
      },
      daysLabels: function(){
          return _.map(this.days,function(d){
                            return (new Date(d.day)).toLocaleDateString();
                        });
      },
      daysOptions: function(){
          return {
            scales: {
                yAxes: [{
                    ticks: {
                        stepSize: 1,
                        min: 0
                    }
                }]
            },
            title:{
                display: false,
                text: 'Incomplete Assignments, Last 30 Days'
            },
            legend:{
                display: false
            }
          };
      },
      usersDataset: function(){
          return [
            {
                label: 'Api/User',
                backgroundColor: _.map(this.users,function(){ return 'rgba('+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+',10)';}),
                data: _.map(this.users,'count'),
                fill:true,
                lineTension: 0
            }
        ];
      },
      usersLabels:function(){
          return _.map(this.users,function(d){
            if(d.user){
                return d.user.email;
            }
            else{
                return "anonymous";
            }
        });
      },
      routesDataset: function(){
          return [
            {
                label: 'Count',
                backgroundColor: _.map(this.routes,function(){ return 'rgba('+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+',10)';}),
                data: _.map(this.routes,'count'),
                fill:true,
                lineTension: 0
            }
        ];
      },
      routeTimeCumulativeDataset: function(){
          return [
            {
                label: 'Cumulative MS',
                backgroundColor: _.map(this.routePeformance,function(){ return 'rgba('+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+','+(Math.floor(Math.random() * Math.floor(255)))+',10)';}),
                data: _.map(this.routePeformance,'sumMs'),
                fill:true,
                lineTension: 0
            }
        ];
      },
      routesLabels:function(){
          return _.map(this.routes,'route');
      },
      routePerformanceDataset: function(){
          return [
            {
                label: 'Min',
                backgroundColor: 'rgba(0,0,255)',
                data: _.map(this.routePeformance,'minMs'),
                fill:true,
                lineTension: 0
            },
            {
                label: 'Avg',
                backgroundColor: 'rgba(255,0,255)',
                data: _.map(this.routePeformance,'avgMs'),
                fill:true,
                lineTension: 0
            },
            {
                label: 'Max',
                backgroundColor: 'rgba(255,0,0)',
                data: _.map(this.routePeformance,'maxMs'),
                fill:true,
                lineTension: 0
            },
        ];
      },
      routePerformanceLabels:function(){
          return _.map(this.routePeformance,'route');
      }
  },
  methods: {
        loadRoutes() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('sysadmin/metrics/routes',{from: this.from,to: this.to,route: this.route})
                .then(response => {
                    console.log(response);
                    this.routes=response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        loadDays() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('sysadmin/metrics/days',{from: this.from,to: this.to,route: this.route})
                .then(response => {
                    console.log(response);
                    this.days=response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        loadUsers() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('sysadmin/metrics/users',{from: this.from,to: this.to,route: this.route})
                .then(response => {
                    console.log(response);
                    this.users=response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        load(){
            this.loadRoutes();
            this.loadDays();
            this.loadUsers();
            this.loadRoutePerformance();
        },
        loadRoutePerformance() {
            this.$store.dispatch('loading','Loading...');
            this.$http.post('sysadmin/performance/routes',{from: this.from,to: this.to,route: this.route})
                .then(response => {
                    console.log(response);
                    this.routePeformance=response.data;
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        }
  },
  watch: {
    from: function(){
        this.load();
    },
    to: function(){
        this.load();
    },
    route: function(){
        this.load();
    }
  }
}
</script>
