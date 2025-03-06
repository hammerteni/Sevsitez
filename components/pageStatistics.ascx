<%@ Control %>

<script type="text/javascript" src="/components/vue/vue.min.js"></script>
<link rel="stylesheet" href="/components/bootstrap-vue/bootstrap-vue.css"/>
<link rel="stylesheet" href="/components/bootstrap/bootstrap.css"/>
<script src="/Scripts/axios/axios.min.js"></script>

<div id="pageStatistics">
   <b-jumbotron>
        <h3>Статистика по заявкам конкурсов <b-button variant="primary" v-on:click="getData">Рассчитать статистику</b-button> </h3>

         <b-table 
            :items="statData" 
            :busy="isBusy" 
            :bordered="true"
            :small="true"
            :hover="true"
            :head-variant="'dark'"
            :table-variant="'warning'"
            class="m-t-3">
          <template v-slot:table-busy>
            <div class="text-center text-danger my-2">
              <b-spinner class="align-middle"></b-spinner>
              <strong>Загрузка...</strong>
            </div>
          </template>
        </b-table>

      
        <b-table 
            :items="statDataUchr" 
            :busy="isBusy" 
            :bordered="true"
            :small="true"
            :hover="true"
            :head-variant="'dark'"
            :table-variant="'danger'"
            class="m-t-3">
         <template v-slot:table-busy>
          <div class="text-center text-danger my-2">
            <b-spinner class="align-middle"></b-spinner>
            <strong>Загрузка...</strong>
          </div>
        </template>
        </b-table>
   
       <!--
       <div v-if="unknowData.length > 0">
           <h3>Нераспознанные заявки по регионам <strong>{{unknowData.length}}</strong></h3>
           <b-table 
                :items="unknowData" 
                :bordered="true"
                :small="true"
                :hover="true"
                :head-variant="'dark'"
                :table-variant="'danger'"
                class="m-t-3">
            </b-table>
       </div>
       <div v-if="unknownMoscowData.length > 0">
           <h3>Нераспознанные московские заявки <strong>{{unknownMoscowData.length}}</strong></h3>
           <b-table 
                :items="unknownMoscowData" 
                :bordered="true"
                :small="true"
                :hover="true"
                :head-variant="'dark'"
                :table-variant="'danger'"
                class="m-t-3">
            </b-table>
       </div>
        -->
  </b-jumbotron>
</div>

<script type="text/javascript" src="/components/bootstrap-vue/bootstrap-vue.js"></script>
<script>
    var vueApp = new Vue({
        el: "#pageStatistics",
        data: {
            isBusy: false,
            emptyArr1: [{ 'Показатель': ' ', 'Количественный_показатель': ' ' }],
            emptyArr2: [{ 'Показатель': ' ', 'Количество_учреждений': ' ', 'Количество_участников_I_тура': ' ', 'Количество_участников_II_тура': ' ', 'Общее_количество_участников': ' ' }],
            statData: function () {
                return vueApp.emptyArr1;
            },
            statDataUchr: function () {
                return vueApp.emptyArr2;
            }
        },
        methods: {
            loadPage: function () {
                console.log('load page')
            },
            getData: function () {
                vueApp.isBusy = true;
                return axios({
                    method: 'get',
                    url: '/api/statistics',
                }).then(function (result) {
                    vueApp.statData = result.data.statData;
                    vueApp.statDataUchr = result.data.statDataUchr;
                    vueApp.isBusy = false;
                }).catch(function (err) {
                    console.log(err);
                    vueApp.isBusy = false;
                });
            }
        },
        created: function () {
            this.loadPage();
        }
    });

</script>
