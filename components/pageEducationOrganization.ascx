<%@ Control %>

<script type="text/javascript" src="/components/vue/vue.min.js"></script>
<link rel="stylesheet" href="/components/bootstrap-vue/bootstrap-vue.css"/>
<link rel="stylesheet" href="/components/bootstrap/bootstrap.css"/>
<script src="/components/bootstrap/bootstrap.min.js"></script>
<script src="/Scripts/axios/axios.min.js"></script>
<script type="text/javascript" src="/components/bootstrap-vue/bootstrap-vue.js"></script>

<div class="p-3" id="OrgListTemplate">

    <div class="mt-5 row">
        <div class="col-md-7">
            <h4>Список образовательных учреждений</h4> 
        </div>
        <div class="col-md-5 text-right">
            <button type="button" class="btn btn-success" @click="showModal('add')" >Добавить</button>
        </div>
    </div>
    <div class="list row mt-1">
        <div class="col-md-7">
            <div class="input-group mb-3">
              <input
                type="text"
                class="form-control"
                placeholder="Найти образовательное учреждение"
                v-model="searchString"
                v-on:keyup.enter="onEnter"
              />
              <div class="input-group-append">
                <button
                  class="btn btn-outline-secondary"
                  type="button"
                  @click="page = 1; retrieveTutorials();"
                >
                  Поиск
                </button>
              </div>
            </div>
      </div>
     
  </div> 
  <div class="list row mt-1">
    <div class="col-md-12">
        <div class="mb-3">
            Строк на странице:
            <select v-model="pageSize" @change="handlePageSizeChange($event)">
            <option v-for="size in pageSizes" :key="size" :value="size">
                {{ size }}
            </option>
            </select>
        </div>

        <b-pagination
            v-model="page"
            :total-rows="count"
            :per-page="pageSize"
            prev-text="Пред"
            next-text="След"
            @change="handlePageChange"
        ></b-pagination>
      </div>
    </div>
    <div class="list row mt-1">
        <div class="col-md-6">
       
        <ul class="list-group" id="tutorials-list">
            <li type="button"
            class="list-group-item"
            :class="{ active: index + (page * pageSize) == currentIndex }"
            v-for="(tutorial, index) in tutorials"
            :key="index"
            @click="setActiveTutorial(tutorial, index)"
            >
            <p class="mb-0">{{ tutorial.FullName }}</p><p class="mb-0">{{ tutorial.Name }}</p>
            <p class="mb-0 font-weight-light">{{ tutorial.District }} | {{ tutorial.Region }} | {{ tutorial.Area }}  | {{ tutorial.City }}</p>
            <p class="mb-0 font-weight-light">Тип: {{ tutorial.TypeName }} | МРСД: {{ tutorial.MRSD }}</p>
            </li>
        </ul>

        <%-- <button class="m-3 btn btn-sm btn-danger" @click="removeAllTutorials">
            Remove All
        </button>--%>
        </div>

    <div class="col-md-6">
        <div v-if="currentTutorial">
            <h4>Образовательное учреждение</h4>
            <div>
            <label><strong>Полное наименование:</strong></label> 
                {{ currentTutorial.FullName }}
            </div>
            <div>
            <label><strong>Наименование:</strong></label>
                {{ currentTutorial.Name }}
            </div>
            <div>
                <label><strong>Федеральный округ:</strong></label>
                {{ currentTutorial.District }}
            </div>
            <div>
            <label><strong>Регион:</strong></label>
                {{ currentTutorial.Region }}
            </div>
            <div>
                <label><strong>Район:</strong></label>
                {{ currentTutorial.Area }}
            </div>
            <div>
                <label><strong>Город:</strong></label>
                {{ currentTutorial.City }}
            </div>
            <div>
                <label><strong>Тип:</strong></label>
                {{ currentTutorial.TypeName }}
            </div>
        
            <div>
                <label><strong>МРСД:</strong></label>
                {{ currentTutorial.MRSD }}
            </div>
            <div class="btn-toolbar justify-content-between">
                <div class="btn-group">
                    <button type="button" class="btn btn-warning" @click="showModal('edit')">
                        Изменить
                    </button>
                </div>
                <div class="btn-group">
                    <button type="button" class="btn btn-danger right" @click="deleteTutorial(currentTutorial.FullName, currentTutorial.Name)">
                        Удалить
                    </button>
                </div>
            </div>
        </div>
        <div v-else>
            <br />
            <p>Кликните на найденную строку...</p>
        </div>
    </div>
</div>
 <div class="list row mt-1">
   <div class="col-md-12">
       <div class="mb-3">
           Строк на странице:
           <select v-model="pageSize" @change="handlePageSizeChange($event)">
           <option v-for="size in pageSizes" :key="size" :value="size">
               {{ size }}
           </option>
           </select>
       </div>

       <b-pagination
           v-model="page"
           :total-rows="count"
           :per-page="pageSize"
           prev-text="Пред"
           next-text="След"
           @change="handlePageChange"
       ></b-pagination>
     </div>
   </div>

        <!-- Modal -->
    <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="exampleModalLabel">{{action == 'edit' ? "Изменить" : "Добавить"}} образовательное учреждение</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
           <div class="form-group">
              <label for="FullName">Полное наименование</label>
              <input
                type="text"
                class="form-control"
                id="FullName"
                required
                v-model="modifyModel.FullName"
                name="FullName"
                  :disabled="action == 'edit'"
              />
            </div>

            <div class="form-group">
              <label for="Name">Наименование</label>
              <input
                class="form-control"
                id="Name"
                required
                v-model="modifyModel.Name"
                name="Name"
                  :disabled="action == 'edit'"
              />
            </div>

            <div class="form-group">
                <label for="District">Федеральный округ</label>
                <input
                    class="form-control"
                    id="District"
                    required
                    v-model="modifyModel.District"
                    name="District"
                />
            </div>

            <div class="form-group">
                <label for="Region">Регион</label>
                <input
                    class="form-control"
                    id="Region"
                    required
                    v-model="modifyModel.Region"
                    name="Region"
                />
            </div>

            <div class="form-group">
                <label for="Area">Район</label>
                <input
                    class="form-control"
                    id="Area"
                    required
                    v-model="modifyModel.Area"
                    name="Area"
                />
            </div>

            <div class="form-group">
                <label for="City">Город</label>
            <input
                    class="form-control"
                    id="City"
                    required
                    v-model="modifyModel.City"
                    name="City"
                />
            </div>

               <div class="form-group">
                     <label for="TypeName">Тип образовательного учреждения</label>
                 <input
                         class="form-control"
                         id="TypeName"
                         required
                         v-model="modifyModel.TypeName"
                         name="TypeName"
                     />
                 </div>

               <div class="form-group">
                     <label for="MRSD">МРСД</label>
                 <input
                         class="form-control"
                         id="MRSD"
                         required
                         v-model="modifyModel.MRSD"
                         name="MRSD"
                     />
                 </div>

          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Закрыть</button>
            <button @click.prevent="saveTutorial(action)" class="btn btn-success">Сохранить</button>
          </div>
        </div>
      </div>
    </div>

</div>

<script>
    
    Vue.use(BootstrapVue);

    new Vue({
        data() {
            return {
                tutorials: [],
                currentTutorial: null,
                
                action: "",
                modifyModel: {},

                currentIndex: -1,
                searchString: "",

                page: 1,
                count: 0,
                pageSize: 10,

                pageSizes: [10, 50, 100],

                processing: false
            }
        },
        methods: {
            getRequestParams(searchString, page, pageSize) {
                let params = {};

                params["searchString"] = searchString;

                if (page) {
                    params["page"] = page - 1;
                }

                if (pageSize) {
                    params["size"] = pageSize;
                }

                return params;
            },

            retrieveTutorials() {
                const params = this.getRequestParams(
                    this.searchString,
                    this.page,
                    this.pageSize
                );

                axios.get('/api/EducationOrganizations', { params: params })
                    .then((response) => {
                        const { educationOrganizations, countItems } = response.data;
                        this.tutorials = educationOrganizations;
                        this.count = countItems;
                        //console.log(params)
                    })
                    .catch((e) => {
                        console.log(e);
                        if (e.response.data.Message)
                            alert(e.response.data.Message);
                        else
                            alert(e.Message); 
                    });
            },

            handlePageChange(value) {
                this.page = value;
                this.retrieveTutorials();
            },

            handlePageSizeChange(event) {
                this.pageSize = event.target.value;
                this.page = 1;
                this.retrieveTutorials();
            },

            refreshList() {
                this.retrieveTutorials();
                this.currentTutorial = null;
                this.currentIndex = -1;
            },

            setActiveTutorial(tutorial, index) {
                this.currentTutorial = tutorial;
                this.currentIndex = index + (this.page * this.pageSize);
            },
            removeAllTutorials() {
                axios.delete('/api/EducationOrganizations')
                    .then((response) => {
                        this.refreshList();
                    })
                    .catch((e) => {
                        console.log(e);
                        if (e.response.data.Message)
                            alert(e.response.data.Message);
                        else
                            alert(e.Message); 
                    });
            },
            saveTutorial(action) {

                if (this.processing === true) {
                    return;
                } 

                this.processing = true;

                var data = {
                    FullName: this.modifyModel.FullName,
                    Name: this.modifyModel.Name,
                    District: this.modifyModel.District,
                    Region: this.modifyModel.Region,
                    Area: this.modifyModel.Area,
                    City: this.modifyModel.City,
                    TypeName: this.modifyModel.TypeName,
                    MRSD: this.modifyModel.MRSD,
                };

                if (action == 'add') {
                    axios.post('/api/EducationOrganizations/AddEducationOrganizations', data)
                        .then((response) => {
                            this.retrieveTutorials()
                            this.hideModal();
                            this.processing = false;
                        })
                        .catch((e) => {
                            this.processing = false;
                            console.log(e);
                            if (e.response.data.Message)
                                alert(e.response.data.Message);
                            else
                                alert(e.Message); 
                            //this.hideModal();
                        });
                }
                else {
                    axios.post('/api/EducationOrganizations/ModifyEducationOrganizations?fullName=' + this.currentTutorial.FullName + '&shortName=' + this.currentTutorial.Name, data)
                        .then((response) => {
                            this.retrieveTutorials()
                            this.currentTutorial = data;
                            this.hideModal();
                            this.processing = false;
                        })
                        .catch((e) => {
                            this.processing = false;
                            console.log(e);
                            if (e.response.data.Message)
                                alert(e.response.data.Message);
                            else
                                alert(e.Message); 
                            //this.hideModal();
                        });
                }
            },
            deleteTutorial(fullname, shortname) {
                if (window.confirm("Вы уверены, что хотите удалить образовательное учреждение?")) {
                    
                    axios.post('/api/EducationOrganizations/DeleteEducationOrganizations?fullName=' + fullname + '&shortName=' + shortname)
                        .then((response) => {
                            if (response.data.status == "Error")
                                alert(response.data.message);
                            else {
                                this.refreshList();
                            }
                        })
                        .catch((e) => {
                            console.log(e);
                            if (e.response.data.Message)
                                alert(e.response.data.Message);
                            else
                                alert(e.Message); 
                        });
                }
            },
            showModal: function (action) {
                this.action = action;
                if (action == 'add')
                    this.modifyModel = {};
                else
                    this.modifyModel = JSON.parse(JSON.stringify(this.currentTutorial));

                $('#exampleModal').modal('show');
            },
            hideModal: function () {
                $('#exampleModal').modal('hide');
            },
            onEnter: function () {
                this.retrieveTutorials();
            }
        },
        mounted() {
            this.retrieveTutorials();
           
        },
    }).$mount('#OrgListTemplate')

</script>
