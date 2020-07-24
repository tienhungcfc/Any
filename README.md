# Any
> Dữ liệu động, theo cây
###### Sử dụng cơ bản
```c#
dynamic a = new Any(); /// {}
a.a = 0; /// {"a": 0}
a.a = "title"; /// {"a": "title"}
a.a = true; /// {"a": true}
a.a = new object[] {0,1}; /// {"a": [0,1]}
a.a[0].title = "tille a"; /// {"a": [{"title": "title a"}, 1]}
a.a = 1000; /// {"a": 1000}
a.a.b.c = 0; /// Không hợp lệ Kq vẫn là {"a": 1000}
a.a.b = 100; /// {"a": { "a": {"b": 100}}}
```
###### Truy vấn theo key
```c#
...
a.a = new object[] {0,1}; /// {"a": [0,1]}
Any.Select((Any)a,"a", (Any x,int i) {  
  if(i==1) x.append1 = "append for 1"; return 0; 
}); /// {"a": [{"title": "title a"}, {"append1": "append for 1"}]}
```

