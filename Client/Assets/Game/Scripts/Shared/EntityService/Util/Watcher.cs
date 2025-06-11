using System.IO;

namespace EntityService.Util {
    public class Watcher {
        private readonly FileSystemWatcher m_watcher = new FileSystemWatcher();
        public string WatchPath { get; private set; }

        public void Init(string path, string filter = "*.csv")
        {
            WatchPath = path;

            // 조사할 디렉터리의 경로를 가져오거나 설정
            m_watcher.Path = WatchPath;

            // 조사할 변경 내용 형식을 가져오거나 설정
            m_watcher.NotifyFilter = NotifyFilters.LastWrite
                                   | NotifyFilters.FileName;

            // 디렉터리에서 모니터링할 파일을 결정하는 데 사용되는 필터 문자열을 가져오거나 설정(*.* 모든 파일)
            m_watcher.Filter = filter;

            // 지정된 경로 내에 있는 하위 디렉터리를 모니터링해야 하는지를 나타내는 값을 가져오거나 설정
            m_watcher.IncludeSubdirectories = true;

            // 이벤트 설정
            //m_watcher.Created -= OnCreated;
            //m_watcher.Created += OnCreated;

            m_watcher.Changed -= OnChanged;
            m_watcher.Changed += OnChanged;

            //m_watcher.Deleted -= OnDeleted;
            //m_watcher.Deleted += OnDeleted;

            //m_watcher.Renamed -= OnRenamed;
            //m_watcher.Renamed += OnRenamed;

            // 구성 요소가 활성화되는지를 나타내는 값을 가져오거나 설정합니다.
            m_watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {

        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var name = Path.GetFileNameWithoutExtension(e.FullPath);
            ESTableManager.Drop(name);
            ESTableManager.LoadFile(e.FullPath, ".csv");
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {

        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {

        }
    }
}
