using System.Collections.Generic;

namespace PSProxmox.Models
{
    /// <summary>
    /// Builder class for creating Proxmox LXC containers
    /// </summary>
    public class ProxmoxContainerBuilder
    {
        private readonly Dictionary<string, object> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxmoxContainerBuilder"/> class
        /// </summary>
        /// <param name="name">The container name</param>
        public ProxmoxContainerBuilder(string name)
        {
            _parameters = new Dictionary<string, object>
            {
                { "hostname", name }
            };
        }

        /// <summary>
        /// Gets the parameters for creating the container
        /// </summary>
        public Dictionary<string, object> Parameters => _parameters;

        /// <summary>
        /// Sets the container OS template
        /// </summary>
        /// <param name="ostemplate">The OS template</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithOSTemplate(string ostemplate)
        {
            _parameters["ostemplate"] = ostemplate;
            return this;
        }

        /// <summary>
        /// Sets the container storage
        /// </summary>
        /// <param name="storage">The storage name</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithStorage(string storage)
        {
            _parameters["storage"] = storage;
            return this;
        }

        /// <summary>
        /// Sets the container memory limit
        /// </summary>
        /// <param name="memory">The memory limit in MB</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithMemory(int memory)
        {
            _parameters["memory"] = memory;
            return this;
        }

        /// <summary>
        /// Sets the container swap limit
        /// </summary>
        /// <param name="swap">The swap limit in MB</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithSwap(int swap)
        {
            _parameters["swap"] = swap;
            return this;
        }

        /// <summary>
        /// Sets the container CPU cores
        /// </summary>
        /// <param name="cores">The number of CPU cores</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithCores(int cores)
        {
            _parameters["cores"] = cores;
            return this;
        }

        /// <summary>
        /// Sets the container disk size
        /// </summary>
        /// <param name="diskSize">The disk size in GB</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithDiskSize(int diskSize)
        {
            _parameters["rootfs"] = $"local-lvm:{diskSize}";
            return this;
        }

        /// <summary>
        /// Sets the container disk size with specific storage
        /// </summary>
        /// <param name="storage">The storage name</param>
        /// <param name="diskSize">The disk size in GB</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithDiskSize(string storage, int diskSize)
        {
            _parameters["rootfs"] = $"{storage}:{diskSize}";
            return this;
        }

        /// <summary>
        /// Sets the container to be unprivileged
        /// </summary>
        /// <param name="unprivileged">Whether the container is unprivileged</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithUnprivileged(bool unprivileged)
        {
            _parameters["unprivileged"] = unprivileged ? 1 : 0;
            return this;
        }

        /// <summary>
        /// Sets the container password
        /// </summary>
        /// <param name="password">The container password</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithPassword(string password)
        {
            _parameters["password"] = password;
            return this;
        }

        /// <summary>
        /// Sets the container SSH public key
        /// </summary>
        /// <param name="sshKey">The SSH public key</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithSSHKey(string sshKey)
        {
            _parameters["ssh-public-keys"] = sshKey;
            return this;
        }

        /// <summary>
        /// Sets the container network interface
        /// </summary>
        /// <param name="name">The interface name</param>
        /// <param name="bridge">The bridge name</param>
        /// <param name="ip">The IP address</param>
        /// <param name="gateway">The gateway address</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithNetwork(string name, string bridge, string ip, string gateway)
        {
            _parameters[$"net{name}"] = $"name=eth0,bridge={bridge},ip={ip},gw={gateway}";
            return this;
        }

        /// <summary>
        /// Sets the container description
        /// </summary>
        /// <param name="description">The container description</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithDescription(string description)
        {
            _parameters["description"] = description;
            return this;
        }

        /// <summary>
        /// Sets the container start on boot flag
        /// </summary>
        /// <param name="start">Whether to start the container on boot</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithStartOnBoot(bool start)
        {
            _parameters["onboot"] = start ? 1 : 0;
            return this;
        }

        /// <summary>
        /// Sets the container start after creation flag
        /// </summary>
        /// <param name="start">Whether to start the container after creation</param>
        /// <returns>The builder instance</returns>
        public ProxmoxContainerBuilder WithStart(bool start)
        {
            _parameters["start"] = start ? 1 : 0;
            return this;
        }
    }
}
